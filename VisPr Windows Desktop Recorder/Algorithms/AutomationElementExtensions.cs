using FlaUI.Core.AutomationElements;
using System;
using System.Collections.Generic;

namespace VisPrWindowsDesktopRecorder.Algorithms
{
    public static class AutomationElementExtensions
    {
        public static bool IsTextElement(this AutomationElement element) 
        {
            return 
                element.Patterns.Text.IsSupported ||
                element.Patterns.Text2.IsSupported ||
                element.Patterns.TextEdit.IsSupported ||
                element.Patterns.TextChild.IsSupported;
        }

        public static List<ElementSelector> Seletors(this AutomationElement e)
        {
            var r = new List<ElementSelector>();
            try
            {
                if (e == null)
                {
                    return r;
                }

                if (e.Properties.AutomationId.IsSupported)
                {
                    var p = e.Properties.AutomationId;
                    r.Add(new ElementSelector { Property = "AutomationId", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault });
                }

                if (e.Properties.Name.IsSupported)
                {
                    var p = e.Properties.Name;
                    r.Add(new ElementSelector { Property = "Name", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault });
                }

                if (e.Properties.AcceleratorKey.IsSupported)
                {
                    var p = e.Properties.AcceleratorKey;
                    r.Add(new ElementSelector { Property = "AcceleratorKey", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault });
                }

                if (e.Properties.AccessKey.IsSupported)
                {
                    var p = e.Properties.AccessKey;
                    var el = new ElementSelector { Property = "AccessKey", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault };
                    var v = el.Value as string;
                    if (!string.IsNullOrEmpty(v))
                    {
                        r.Add(el);
                    }
                }

                if (e.Properties.BoundingRectangle.IsSupported)
                {
                    var p = e.Properties.BoundingRectangle;
                    r.Add(new ElementSelector { Property = "BoundingRectangle", DataType = SelectorDataType.RECT, Value = p.ValueOrDefault });
                }

                if (e.Properties.ClassName.IsSupported)
                {
                    var p = e.Properties.ClassName;
                    r.Add(new ElementSelector { Property = "ClassName", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault });
                }

                if (e.Properties.ControlType.IsSupported)
                {
                    var p = e.Properties.ControlType;
                    r.Add(new ElementSelector { Property = "ControlType", DataType = SelectorDataType.CONTROL_TYPE, Value = p.ValueOrDefault });
                }

                if (e.Properties.FrameworkId.IsSupported)
                {
                    var p = e.Properties.FrameworkId;
                    r.Add(new ElementSelector { Property = "FrameworkId", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.IsDialog.IsSupported)
                {
                    var p = e.Properties.IsDialog;
                    r.Add(new ElementSelector { Property = "IsDialog", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.IsEnabled.IsSupported)
                {
                    var p = e.Properties.IsEnabled;
                    r.Add(new ElementSelector { Property = "IsEnabled", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.ItemStatus.IsSupported)
                {
                    var p = e.Properties.ItemStatus;
                    r.Add(new ElementSelector { Property = "ItemStatus", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault });
                }

                if (e.Properties.IsPassword.IsSupported)
                {
                    var p = e.Properties.IsPassword;
                    r.Add(new ElementSelector { Property = "IsPassword", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.IsRequiredForForm.IsSupported)
                {
                    var p = e.Properties.IsRequiredForForm;
                    r.Add(new ElementSelector { Property = "IsRequiredForForm", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.IsPeripheral.IsSupported)
                {
                    var p = e.Properties.IsPeripheral;
                    r.Add(new ElementSelector { Property = "IsPeripheral", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.CenterPoint.IsSupported)
                {
                    var p = e.Properties.CenterPoint;
                    r.Add(new ElementSelector { Property = "CenterPoint", DataType = SelectorDataType.POINT, Value = p.ValueOrDefault });
                }

                if (e.Properties.Culture.IsSupported)
                {
                    var p = e.Properties.Culture;
                    r.Add(new ElementSelector { Property = "Culture", DataType = SelectorDataType.CULTURE_INFO, Value = p.ValueOrDefault });
                }

                if (e.Properties.FillColor.IsSupported)
                {
                    var p = e.Properties.FillColor;
                    r.Add(new ElementSelector { Property = "FillColor", DataType = SelectorDataType.INT, Value = p.ValueOrDefault });
                }

                if (e.Properties.FullDescription.IsSupported)
                {
                    var p = e.Properties.FullDescription;
                    r.Add(new ElementSelector { Property = "FullDescription", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault });
                }

                if (e.Properties.HasKeyboardFocus.IsSupported)
                {
                    var p = e.Properties.HasKeyboardFocus;
                    r.Add(new ElementSelector { Property = "HasKeyboardFocus", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.HeadingLevel.IsSupported)
                {
                    var p = e.Properties.HeadingLevel;
                    r.Add(new ElementSelector { Property = "HeadingLevel", DataType = SelectorDataType.HEADING_LEVEL, Value = p.ValueOrDefault });
                }

                if (e.Properties.HelpText.IsSupported)
                {
                    var p = e.Properties.HelpText;
                    var text = p.ValueOrDefault;
                    if (!string.IsNullOrEmpty(text))
                    {
                        r.Add(new ElementSelector { Property = "HelpText", DataType = SelectorDataType.STRING, Value = text.Trim() });
                    }
                }

                if (e.Properties.IsContentElement.IsSupported)
                {
                    var p = e.Properties.IsContentElement;
                    r.Add(new ElementSelector { Property = "IsContentElement", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.IsControlElement.IsSupported)
                {
                    var p = e.Properties.IsControlElement;
                    r.Add(new ElementSelector { Property = "IsControlElement", DataType = SelectorDataType.BOOL, Value = p.ValueOrDefault });
                }

                if (e.Properties.Level.IsSupported)
                {
                    var p = e.Properties.Level;
                    r.Add(new ElementSelector { Property = "Level", DataType = SelectorDataType.INT, Value = p.ValueOrDefault });
                }

                if (e.Properties.AnnotationTypes.IsSupported)
                {
                    var p = e.Properties.AnnotationTypes;
                    r.Add(new ElementSelector { Property = "AnnotationTypes", DataType = SelectorDataType.INT_ARRAY, Value = p.ValueOrDefault });
                }

                if (e.Properties.AnnotationObjects.IsSupported)
                {
                    var p = e.Properties.AnnotationObjects;
                    r.Add(new ElementSelector { Property = "AnnotationObjects", DataType = SelectorDataType.INT_ARRAY, Value = p.ValueOrDefault });
                }

                if (e.Properties.AriaProperties.IsSupported)
                {
                    var p = e.Properties.AriaProperties;
                    r.Add(new ElementSelector { Property = "AriaProperties", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault });
                }

                if (e.Properties.AriaRole.IsSupported)
                {
                    var p = e.Properties.AriaRole;
                    r.Add(new ElementSelector { Property = "AriaRole", DataType = SelectorDataType.STRING, Value = p.ValueOrDefault });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return r;
        }
    }
}
