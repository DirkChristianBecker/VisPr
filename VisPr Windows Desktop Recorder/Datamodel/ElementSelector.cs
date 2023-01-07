using System;

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
    }
}
