using FlaUI.Core;
using FlaUI.Core.Conditions;
using FlaUI.Core.Identifiers;
using FlaUI.UIA2;
using FlaUI.UIA2.Identifiers;
using FlaUI.UIA3;
using Microsoft.VisualBasic.ApplicationServices;
using System.Runtime.CompilerServices;
using System.Windows.Automation;
using VisPrCore.Datamodel.Requests.Runtime;
using VisPrCore.Datamodel;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;
using VisPrCore.Datamodel.Responses.Runtime;
using VisPrRuntime.Services;
using System.Windows.Forms.Design;
using FlaUI.Core.AutomationElements;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;

namespace VisPrRuntime.Services
{
    public interface IQueryApplication
    {
        /// <summary>
        /// Get a running application.
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        (AutomationBase, Application) this[int id] { get; }

        /// <summary>
        /// Launch an application and initialize a automation object.
        /// 
        /// </summary>
        /// <param name="t">Automation type to use</param>
        /// <param name="path">Executable path</param>
        /// <param name="arguments">Startup arguments</param>
        /// <returns>The an application id</returns>
        int Launch(ElementType t, string path, string? arguments);

        /// <summary>
        /// Terminate an application. If the timeout is set the application will be forcefully terminated
        /// after waiting for the given time.
        /// Returns true, if the application could be gracefully terminated. False otherwise.
        /// 
        /// </summary>
        /// <param name="id">Application id</param>
        /// <param name="timeOut">Timeout</param>
        /// <returns>True, if the application could be gracefully terminated.</returns>
        bool Terminate(int id, TimeSpan? timeOut);

        /// <summary>
        /// List all elements on application level.
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        QueryRuntimeResponse ListApplicationModel(int id);

        /// <summary>
        /// List all direct children below the selected element.
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="t"></param>
        /// <param name="selectors"></param>
        /// <returns></returns>
        QueryRuntimeResponse ListChildren(int app, ElementType t, List<ElementSelector> selectors);

        /// <summary>
        /// Retrieve a list of all property ids VisPr² knows.
        /// 
        /// </summary>
        /// <returns></returns>
        public ListPropertiesResponse ListProperties();

        /// <summary>
        /// Waits until the element has a clickable point.
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="t"></param>
        /// <param name="selectors"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<UiElementDescriptor> WaitUntilClickable(int app, ElementType t, List<ElementSelector> selectors, TimeSpan timeout);

        /// <summary>
        /// Waits until the element is enabled.
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="t"></param>
        /// <param name="selectors"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<UiElementDescriptor> WaitUntilEnabled(int app, ElementType t, List<ElementSelector> selectors, TimeSpan timeout);

        /// <summary>
        /// Draw a highlight around the element with the given settings.
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="t"></param>
        /// <param name="selectors"></param>
        /// <param name="color">The color to draw the highlight.</param>
        /// <param name="duration">The duration how long the highlight is shown.</param>
        /// <returns></returns>
        public Task<UiElementDescriptor> DrawHighlight(
            int app, 
            ElementType t, 
            List<ElementSelector> selectors, 
            Color? color, 
            TimeSpan? duration = null);

    }

    public class QueryApplication : IQueryApplication
    {
        /// <summary>
        /// A counter that allows us to assign a unique id to each application.
        /// 
        /// </summary>
        private int NextAppId { get; set; }

        /// <summary>
        /// All launched applications ordered by their id.
        /// 
        /// </summary>
        private Dictionary<int, Application> Applications { get; set; }

        /// <summary>
        /// All launched applications ordered by their id.
        /// 
        /// </summary>
        private Dictionary<int, AutomationBase> Automations { get; set; }

        /// <summary>
        /// For atomic operations.
        /// 
        /// </summary>
        private readonly object Lock = new object();

        /// <summary>
        /// A logger
        /// 
        /// </summary>
        private ILogger<QueryApplication> Logger { get; set; }

        private Dictionary<string, PropertyId> UiA2Properties { get; set; }
        private Dictionary<string, PropertyId> UiA3Properties { get; set; }

        public QueryApplication(ILogger<QueryApplication> log)
        {
            Logger = log;
            Applications = new Dictionary<int, Application>();
            Automations = new Dictionary<int, AutomationBase>();
            UiA2Properties = GetUi2Properties();
            UiA3Properties = GetUi3Properties();
        }

        private Dictionary<string, PropertyId> GetUi2Properties()
        {
            var t = typeof(FlaUI.UIA2.Identifiers.AutomationObjectIds);
            if(t == null)
            {
                Logger.Log(LogLevel.Information, $"Type not found.");
                return new Dictionary<string, PropertyId>();
            }

            var r = new Dictionary<string, PropertyId>();
            foreach(var p in t.GetFields())
            {
                var value = p.GetValue(null) as PropertyId;
                if(value == null)
                {
                    Logger.Log(LogLevel.Information, "No value for " + p.Name);
                    continue;
                }

                r.Add(value.Name, value);
                Logger.Log(LogLevel.Information, "Found property: " + value.Name);
            }

            return r;
        }

        private Dictionary<string, PropertyId> GetUi3Properties()
        {
            var t = typeof(FlaUI.UIA3.Identifiers.AutomationObjectIds);
            if (t == null)
            {
                Logger.Log(LogLevel.Information, $"Type not found.");
                return new Dictionary<string, PropertyId>();
            }

            var r = new Dictionary<string, PropertyId>();
            foreach (var p in t.GetFields())
            {
                var value = p.GetValue(null) as PropertyId;
                if (value == null)
                {
                    Logger.Log(LogLevel.Information, "No value for " + p.Name);
                    continue;
                }

                r.Add(value.Name, value);
                Logger.Log(LogLevel.Information, "Found property: " + value.Name);
            }

            return r;
        }

        ~QueryApplication()
        {
            foreach(var a in Automations)
            {
                a.Value.Dispose();
            }

            foreach(var a in Applications)
            {
                a.Value.Dispose();
            }
        }

        public (AutomationBase, Application) this[int id]
        {
            get
            {
                if (!Automations.ContainsKey(id))
                {
                    throw new Exception("Invalid application id.");
                }

                return (Automations[id], Applications[id]);
            }
        }

        public int Launch(ElementType t, string path, string? arguments)
        {
            lock(Lock)
            {
                var app = Application.Launch(path, arguments);
                Logger.Log(LogLevel.Information, $"Launching {path}.");
                if (app == null)
                {
                    throw new Exception("Could not start application.");
                }

                switch (t)
                {
                    case ElementType.UiA2:
                        {
                            Applications[NextAppId] = app;
                            Automations[NextAppId] = new UIA2Automation();
                            app.GetMainWindow(Automations[NextAppId]);

                            var r = NextAppId;
                            NextAppId++;

                            return r;
                        }

                    case ElementType.UiA3:
                        {
                            Applications[NextAppId] = app;
                            Automations[NextAppId] = new UIA3Automation();
                            app.GetMainWindow(Automations[NextAppId]);

                            var r = NextAppId;
                            NextAppId++;

                            return r;
                        }

                    default:
                        throw new Exception("Unsupported element type");

                }
            }
        }

        public bool Terminate(int id, TimeSpan? timeOut)
        {
            lock(Lock)
            {
                Logger.Log(LogLevel.Information, $"Terminating {id}.");

                if (!Automations.ContainsKey(id))
                {
                    throw new Exception("Invalid application id.");
                }

                Applications[id].CloseTimeout = timeOut == null ? new TimeSpan(0, 0, 1) : (TimeSpan) timeOut;
                var r = Applications[id].Close(timeOut != null);

                Applications[id].Dispose();
                Automations[id].Dispose();

                Applications.Remove(id);
                Automations.Remove(id);

                return r;
            }
        }

        public QueryRuntimeResponse ListApplicationModel(int id)
        {
            if (!Automations.ContainsKey(id))
            {
                throw new Exception("Invalid application id.");
            }

            var r = new List<UiElementDescriptor>();
            foreach(var a in Applications[id].GetMainWindow(Automations[id]).FindAllChildren())
            {
                r.Add(UiElementDescriptorExtensions.From(a));
            }

            var response = new QueryRuntimeResponse();
            response.Elements = r;

            return response;
        }

        private FlaUI.Core.AutomationElements.AutomationElement? FindElement(int app, ElementType t, List<ElementSelector> selectors)
        {
            if (!Automations.ContainsKey(app))
            {
                throw new Exception("Invalid application id.");
            }

            var conditions = new ConditionBase[selectors.Count];
            for (int i = 0; i < selectors.Count; i++)
            {
                var selector = selectors[i];
                var id = MapProperty(t, selector);
                switch (selector.DataType)
                {
                    case DataType.Bool:
                        {
                            var value = Boolean.Parse(selector.Value);
                            conditions[i] = id.GetCondition(value);
                            break;
                        }
                    case DataType.Integer:
                        {
                            var value = Int32.Parse(selector.Value);
                            conditions[i] = id.GetCondition(value);
                            break;
                        }
                    case DataType.Double:
                        {
                            var value = Double.Parse(selector.Value);
                            conditions[i] = id.GetCondition(value);
                            break;
                        }
                    case DataType.Float:
                        {
                            var value = float.Parse(selector.Value);
                            conditions[i] = id.GetCondition(value);
                            break;
                        }
                    case DataType.String:
                        {
                            conditions[i] = id.GetCondition(selector.Value);
                            break;
                        }
                }
            }

            var condition = new FlaUI.Core.Conditions.AndCondition(conditions);
            var automation = Applications[app].GetMainWindow(Automations[app]);

            var r = new QueryRuntimeResponse();
            return automation.FindFirst(FlaUI.Core.Definitions.TreeScope.Descendants, condition);  
        }

        public QueryRuntimeResponse ListChildren(int app, ElementType t, List<ElementSelector> selectors)
        {
            var parent = FindElement(app, t, selectors);
            if (parent == null) 
            {
                throw new Exception("UI element not found.");
            }

            var r = new QueryRuntimeResponse();
            foreach (var c in parent.FindAllChildren())
            {
                r.Elements.Add(UiElementDescriptorExtensions.From(c));
            }

            return r;
        }

        private PropertyId MapProperty(ElementType t, ElementSelector s)
        {
            if(t == ElementType.UiA2)
            {
                PropertyId? id;
                if(!UiA2Properties.TryGetValue(s.PropertyName, out id))
                {
                    throw new Exception($"A property {s.PropertyName} does not exist");
                }

                return id;
            }
            
            if(t == ElementType.UiA3) 
            {
                PropertyId? id;
                if (!UiA3Properties.TryGetValue(s.PropertyName, out id))
                {
                    throw new Exception($"A property {s.PropertyName} does not exist");
                }

                return id;
            }

            throw new Exception("Unsupported API-Type");
        }

        public ListPropertiesResponse ListProperties()
        {
            var r = new ListPropertiesResponse();
            foreach (var a in UiA2Properties)
            {
                r.UiA2Properties.Add(a.Value.Name);
            }

            foreach (var a in UiA3Properties)
            {
                r.UiA3Properties.Add(a.Value.Name);
            }

            return r;
        }

        public Task<UiElementDescriptor> DrawHighlight(int app, ElementType t, List<ElementSelector> selectors, Color? color, TimeSpan? duration = null)
        {
            var element = FindElement(app, t, selectors);
            if (element == null)
            {
                throw new Exception("UI element not found.");
            }

            return Task.Run(() => UiElementDescriptorExtensions.From(element.DrawHighlight(false, color ?? Color.Red, duration ?? TimeSpan.FromSeconds(2))));
        }

        public async Task<UiElementDescriptor> WaitUntilClickable(int app, ElementType t, List<ElementSelector> selectors, TimeSpan timeout)
        {
            var element = FindElement(app, t, selectors);
            if (element == null)
            {
                throw new Exception("Element not found");
            }

            return await Task.Run(() =>
            {
                var wait = element.WaitUntilEnabled(timeout);
                return UiElementDescriptorExtensions.From(wait);
            });
        }

        public async Task<UiElementDescriptor> WaitUntilEnabled(int app, ElementType t, List<ElementSelector> selectors, TimeSpan timeout)
        {
            var element = FindElement(app, t, selectors);
            if (element == null)
            {
                throw new Exception("Element not found");
            }

            return await Task.Run(() =>
            {
                var wait = element.WaitUntilEnabled(timeout);
                return UiElementDescriptorExtensions.From(wait);
            });
        }
    }
}
