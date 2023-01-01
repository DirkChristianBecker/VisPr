using FlaUI;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA2;
using FlaUI.UIA3;
using System.Windows.Forms;
using VisPrCore.Datamodel;
using VisPrCore.Datamodel.Responses.Runtime;
using VisPrRuntime.Controllers;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;
using VisPrCore.Datamodel.Requests.Runtime;

namespace VisPrRuntime.Services
{
    public static class UiElementDescriptorExtensions
    {
        public static UiElementDescriptor From(AutomationElement e)
        {
            var r = new UiElementDescriptor();
            r.Name = e.Properties.Name.ValueOrDefault;
            r.Width = e.Properties.BoundingRectangle.ValueOrDefault.Width;
            r.Height = e.Properties.BoundingRectangle.ValueOrDefault.Height;
            r.AutomationId = e.Properties.AutomationId.ValueOrDefault;

            r.X = e.Properties.BoundingRectangle.ValueOrDefault.X;
            r.Y = e.Properties.BoundingRectangle.ValueOrDefault.Y;

            r.AcceleratorKey = e.Properties.AcceleratorKey.ValueOrDefault;
            r.AccessKey = e.Properties.AccessKey.ValueOrDefault;
            r.ClassName = e.Properties.ClassName.ValueOrDefault;
            r.ItemType = e.Properties.ItemType.ValueOrDefault;
            r.ControlType = e.Properties.ControlType.ValueOrDefault.ToString();
            r.HelpText = e.Properties.HelpText.ValueOrDefault;
            r.HasKeyboardFocus = e.Properties.HasKeyboardFocus.ValueOrDefault;
            r.NativeWindowHandle = (Int64) e.Properties.NativeWindowHandle.ValueOrDefault;
            r.ProcessId = e.Properties.ProcessId.ValueOrDefault;
            r.IsEnabled = e.Properties.IsEnabled.ValueOrDefault;
            r.IsOffscreen = e.Properties.IsOffscreen.ValueOrDefault;
            r.IsPassword = e.Properties.IsPassword.ValueOrDefault;

            return r;
        }
    }

    public interface IQueryDesktop
    {
        public QueryRuntimeResponse ListElements(ElementType AutomationType);
    }

    public class QueryDesktop : IQueryDesktop
    {
        private ILogger<RuntimeController> Logger { get; set; } 

        private AutomationBase GetAutomation(ElementType AutomationType)
        {
            switch(AutomationType) 
            {
                case ElementType.UiA2: return new UIA2Automation();
                case ElementType.UiA3: return new UIA3Automation();
                default:
                    throw new Exception("No appropriate automation type found to query the desktop.");
            }
        }

        public QueryDesktop(ILogger<RuntimeController> log) 
        {
            Logger = log;
        }

        public QueryRuntimeResponse ListElements(ElementType AutomationType)
        {
            var r = new List<UiElementDescriptor>();

            var Automation = GetAutomation(AutomationType);
            var desktop = Automation.GetDesktop();
            r.Add(UiElementDescriptorExtensions.From(desktop));
            foreach(var c in desktop.FindAllChildren())
            {
                r.Add(UiElementDescriptorExtensions.From(c));
            }

            return new QueryRuntimeResponse { Elements = r };
        }

        
    }
}
