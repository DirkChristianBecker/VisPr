using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrCore.Datamodel
{
    public class UiElementDescriptor
    {
        public string? Name { get; set; }
        public string? AutomationId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string? AcceleratorKey { get; set; }
        public string? AccessKey { get; set; }
        public string? ClassName { get; set; }
        public string? ItemType { get; set; }
        public string? ControlType { get; set; }
        public string? HelpText { get; set; }
        public bool HasKeyboardFocus { get; set; }
        public Int64 NativeWindowHandle { get; set; }
        public int ProcessId { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsOffscreen { get; set; }
        public bool IsPassword { get; set; }


        public UiElementDescriptor() { }

        public override string ToString()
        {
            return $"Automation element: {Name} Id: ({AutomationId}) X: {X} Y: {Y} Width: {Width} Height: {Height}";
        }
    }
}
