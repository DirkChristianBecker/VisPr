using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisPrWindowsDesktopRecorder
{
    public class ElementSelector
    {
        public string Property { get; set; }
        public object Value { get; set; }
        public Type DataType { get; set; }

        public override string ToString()
        {
            return $"{Property} : {Value} ({DataType})";
        }

        public static bool operator ==(ElementSelector left, ElementSelector right)
        {
            if (left is null || right is null)
            {
                return false;
            }

            if (left.Property != right.Property)
            {
                return false;
            }

            if (left.DataType != right.DataType)
            {
                return false;
            }

            if(left.Value is null && right.Value is null) 
            {
                return true;
            }

            if (left.Value is null)
            {
                return false;
            }

            if(left.DataType == typeof(int[]) && right.DataType == typeof(int[])) 
            {
                var l = left.Value as int[];
                var r2 = right.Value as int[];
                if(l.Length != r2.Length)
                {
                    return false;
                }

                for(int i = 0; i < l.Length; i++) 
                {
                    if (l[i] != r2[i]) 
                    {
                        return false;
                    }
                }

                return true;
            }

            var r = ((object) left.Value).Equals((object) right.Value);
            return r;
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            var x = obj as ElementSelector;
            if(x == null) 
            {
                return false;
            }

            return this == x;
        }

        public override int GetHashCode()
        {
            return Property.GetHashCode() & Value.GetHashCode() & DataType.GetHashCode();
        }

        public static bool operator !=(ElementSelector left, ElementSelector right)
        {
            return !(left == right);
        }

        public static List<ElementSelector> From(AutomationElement e)
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
                    r.Add(new ElementSelector { Property = "AutomationId", DataType = typeof(string), Value = p.ValueOrDefault });
                }

                if (e.Properties.Name.IsSupported)
                {
                    var p = e.Properties.Name;
                    r.Add(new ElementSelector { Property = "Name", DataType = typeof(string), Value = p.ValueOrDefault });
                }

                if (e.Properties.AcceleratorKey.IsSupported)
                {
                    var p = e.Properties.AcceleratorKey;
                    r.Add(new ElementSelector { Property = "AcceleratorKey", DataType = typeof(string), Value = p.ValueOrDefault });
                }

                if (e.Properties.AccessKey.IsSupported)
                {
                    var p = e.Properties.AccessKey;
                    var el = new ElementSelector { Property = "AccessKey", DataType = typeof(string), Value = p.ValueOrDefault };
                    if (el.Value.ToString().Length > 0)
                    {
                        r.Add(el);
                    }
                }

                if (e.Properties.BoundingRectangle.IsSupported)
                {
                    var p = e.Properties.BoundingRectangle;
                    r.Add(new ElementSelector { Property = "BoundingRectangle", DataType = typeof(Rectangle), Value = p.ValueOrDefault });
                }

                if (e.Properties.ClassName.IsSupported)
                {
                    var p = e.Properties.ClassName;
                    r.Add(new ElementSelector { Property = "ClassName", DataType = typeof(string), Value = p.ValueOrDefault });
                }

                if (e.Properties.ControlType.IsSupported)
                {
                    var p = e.Properties.ControlType;
                    r.Add(new ElementSelector { Property = "ControlType", DataType = typeof(ControlType), Value = p.ValueOrDefault });
                }

                if (e.Properties.FrameworkId.IsSupported)
                {
                    var p = e.Properties.FrameworkId;
                    r.Add(new ElementSelector { Property = "FrameworkId", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.IsDialog.IsSupported)
                {
                    var p = e.Properties.IsDialog;
                    r.Add(new ElementSelector { Property = "IsDialog", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.IsEnabled.IsSupported)
                {
                    var p = e.Properties.IsEnabled;
                    r.Add(new ElementSelector { Property = "IsEnabled", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.ItemStatus.IsSupported)
                {
                    var p = e.Properties.ItemStatus;
                    r.Add(new ElementSelector { Property = "ItemStatus", DataType = typeof(string), Value = p.ValueOrDefault });
                }

                if (e.Properties.IsPassword.IsSupported)
                {
                    var p = e.Properties.IsPassword;
                    r.Add(new ElementSelector { Property = "IsPassword", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.IsRequiredForForm.IsSupported)
                {
                    var p = e.Properties.IsRequiredForForm;
                    r.Add(new ElementSelector { Property = "IsRequiredForForm", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.IsPeripheral.IsSupported)
                {
                    var p = e.Properties.IsPeripheral;
                    r.Add(new ElementSelector { Property = "IsPeripheral", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.CenterPoint.IsSupported)
                {
                    var p = e.Properties.CenterPoint;
                    r.Add(new ElementSelector { Property = "CenterPoint", DataType = typeof(Point), Value = p.ValueOrDefault });
                }

                if (e.Properties.Culture.IsSupported)
                {
                    var p = e.Properties.Culture;
                    r.Add(new ElementSelector { Property = "Culture", DataType = typeof(CultureInfo), Value = p.ValueOrDefault });
                }

                if (e.Properties.FillColor.IsSupported)
                {
                    var p = e.Properties.FillColor;
                    r.Add(new ElementSelector { Property = "FillColor", DataType = typeof(int), Value = p.ValueOrDefault });
                }

                if (e.Properties.FullDescription.IsSupported)
                {
                    var p = e.Properties.FullDescription;
                    r.Add(new ElementSelector { Property = "FullDescription", DataType = typeof(string), Value = p.ValueOrDefault });
                }

                if (e.Properties.HasKeyboardFocus.IsSupported)
                {
                    var p = e.Properties.HasKeyboardFocus;
                    r.Add(new ElementSelector { Property = "HasKeyboardFocus", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.HeadingLevel.IsSupported)
                {
                    var p = e.Properties.HeadingLevel;
                    r.Add(new ElementSelector { Property = "HeadingLevel", DataType = typeof(HeadingLevel), Value = p.ValueOrDefault });
                }

                if (e.Properties.HelpText.IsSupported)
                {
                    var p = e.Properties.HelpText;
                    var text = p.ValueOrDefault;
                    if(!string.IsNullOrEmpty(text))
                    {
                        r.Add(new ElementSelector { Property = "HelpText", DataType = typeof(string), Value = text.Trim() });
                    }
                }

                if (e.Properties.IsContentElement.IsSupported)
                {
                    var p = e.Properties.IsContentElement;
                    r.Add(new ElementSelector { Property = "IsContentElement", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.IsControlElement.IsSupported)
                {
                    var p = e.Properties.IsControlElement;
                    r.Add(new ElementSelector { Property = "IsControlElement", DataType = typeof(bool), Value = p.ValueOrDefault });
                }

                if (e.Properties.Level.IsSupported)
                {
                    var p = e.Properties.Level;
                    r.Add(new ElementSelector { Property = "Level", DataType = typeof(int), Value = p.ValueOrDefault });
                }

                if (e.Properties.AnnotationTypes.IsSupported)
                {
                    var p = e.Properties.AnnotationTypes;
                    r.Add(new ElementSelector { Property = "AnnotationTypes", DataType = typeof(int[]), Value = p.ValueOrDefault });
                }

                if (e.Properties.AnnotationObjects.IsSupported)
                {
                    var p = e.Properties.AnnotationObjects;
                    r.Add(new ElementSelector { Property = "AnnotationObjects", DataType = typeof(int[]), Value = p.ValueOrDefault });
                }

                if (e.Properties.AriaProperties.IsSupported)
                {
                    var p = e.Properties.AriaProperties;
                    r.Add(new ElementSelector { Property = "AriaProperties", DataType = typeof(string), Value = p.ValueOrDefault });
                }

                if (e.Properties.AriaRole.IsSupported)
                {
                    var p = e.Properties.AriaRole;
                    r.Add(new ElementSelector { Property = "AriaRole", DataType = typeof(string), Value = p.ValueOrDefault });
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }

            return r;
        }

    }
}
