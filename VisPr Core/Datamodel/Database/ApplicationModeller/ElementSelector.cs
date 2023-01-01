using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller
{
    public enum SelectorOperator 
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Null,
        NotNull
    }

    public enum DataType
    {
        None,
        Integer,
        Double,
        Bool,
        String,
        Float,
    }

    public static class SelectorOperatorExtensions
    {
        public static string DisplayString(this SelectorOperator op)
        {
            switch (op) 
            {
                case SelectorOperator.Equals: return "==";
                case SelectorOperator.NotEquals: return "!=";
                case SelectorOperator.GreaterThan: return ">";
                case SelectorOperator.LessThan: return "<";
                case SelectorOperator.GreaterThanOrEqual: return ">=";
                case SelectorOperator.LessThanOrEqual: return "<=";
                case SelectorOperator.Null: return "NULL";
                case SelectorOperator.NotNull: return "NOT NULL";

                default: return "==";
            }
        }
    }

    public class ElementSelector
    {
        [Key, JsonIgnore] public int Id { get; set; }
        [Required] public string PropertyName { get; set; }
        [Required] public string Value { get; set; }
        [Required] public SelectorOperator Operator { get; set; }
        [Required] public DataType DataType { get; set; }

        public ElementSelector() : this("", "") 
        {
        }

        public ElementSelector(string value, string propertyname) 
        {
            Value = value;
            PropertyName = propertyname;
        }
    }
}
