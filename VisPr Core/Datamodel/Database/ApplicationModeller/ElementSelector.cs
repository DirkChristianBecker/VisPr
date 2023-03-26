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
            return op switch
            {
                SelectorOperator.Equals => "==",
                SelectorOperator.NotEquals => "!=",
                SelectorOperator.GreaterThan => ">",
                SelectorOperator.LessThan => "<",
                SelectorOperator.GreaterThanOrEqual => ">=",
                SelectorOperator.LessThanOrEqual => "<=",
                SelectorOperator.Null => "NULL",
                SelectorOperator.NotNull => "NOT NULL",
                _ => "==",
            };
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
