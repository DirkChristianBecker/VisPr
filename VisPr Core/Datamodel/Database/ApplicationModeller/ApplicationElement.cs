using System.ComponentModel.DataAnnotations;

namespace VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller
{
    /// <summary>
    /// Type of application. This identifies the API VisPr² will be using. 
    /// UiA2 and UiA3 are FlaUI modes.
    /// 
    /// </summary>
    public enum ElementType
    {
        UiA2,
        UiA3,
        Selenium,
        Win32
    }

    /// <summary>
    /// Represents a single UI-Element and how to identify it. 
    /// 
    /// </summary>
    public class ApplicationElement
    {
        /// <summary>
        /// Unique id.
        /// 
        /// </summary>
        [Key] public int Id { get; set; }

        /// <summary>
        /// The name of the element
        /// 
        /// </summary>
        [Required] public string Name { get; set; }

        /// <summary>
        /// A place to store hints and informations.
        /// 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The API to use.
        /// 
        /// </summary>
        public ElementType ElementType { get; set; }

        /// <summary>
        /// A list of selectors that identify a UI-Element. 
        /// 
        /// </summary>
        public List<ElementSelector> Selectors { get; set; }

        /// <summary>
        /// Parent element
        /// 
        /// </summary>
        public ApplicationElement? Parent { get; set; }

        /// <summary>
        /// Foreign key
        /// 
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// All child elements
        /// 
        /// </summary>
        public ICollection<ApplicationElement> Children { get; set; }

        /// <summary>
        /// C-Tor
        /// 
        /// </summary>
        public ApplicationElement() : this("")
        {

        }

        /// <summary>
        /// C-Tor
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ApplicationElement(string name)
        {
            Name = name;
            Selectors = new List<ElementSelector>();
            Children = new List<ApplicationElement>();
        }
    }
}
