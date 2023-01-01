using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;

namespace VisPrCore.Datamodel.Database.ApplicationModeller
{
    /// <summary>
    /// Type of application. This identifies the API VisPr² will be using. 
    /// UiA2 and UiA3 are FlaUI modes.
    /// </summary>
    public enum ApplicationType
    {
        FlaUI,
        Selenium,
    }

    /// <summary>
    /// Business objects allow us to reuse UI-Elements across several Processes. This 
    /// helps with maintenace of processes, because there is a central repository of
    /// UI-Elements. That means changing one central element will have an impact on 
    /// all processes that use this element.
    /// 
    /// </summary>
    public class BusinessObject
    {
        /// <summary>
        /// Database id.
        /// </summary>
        [Key] public int Id { get; set; }

        /// <summary>
        /// A unique name.
        /// 
        /// </summary>
        [Required] public string Name { get; set; }

        /// <summary>
        /// (Optional) A description as source of information for developers. 
        /// 
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// User that created this object.
        /// 
        /// </summary>
        [Required, JsonIgnore] public IdentityUser Author { get; set; }

        /// <summary>
        /// Timestamp of creation. Always UTC.
        /// 
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Timestamp of last change. Always UTC.
        /// </summary>
        public DateTime LastChanged { get; set; }

        /// <summary>
        /// (Optional) Path to the executable of the application we are modeling here.
        /// 
        /// </summary>
        public string? ExecutablePath { get; set; }

        /// <summary>
        /// (Optional) Name of the process inside the task manager. 
        /// 
        /// </summary>
        public string? ProcessName { get; set; }

        /// <summary>
        /// Application type. Tells us which API VisPr² is using for automation.
        /// 
        /// </summary>
        public ApplicationType ApplicationType { get; set; }

        /// <summary>
        /// Row version, managed internally. UTC in milliseconds of the last change.
        /// 
        /// </summary>
        public Int64 Version { get; set; }

        /// <summary>
        /// All UI elements.
        /// </summary>
        public List<ApplicationElement> Elements { get; set; }

        /// <summary>
        /// C-Tor
        /// </summary>
        public BusinessObject() : this("", new IdentityUser())
        {
        }

        /// <summary>
        /// C-Tor
        /// </summary>
        /// <param name="name">Process name</param>
        /// <param name="author">Author</param>
        public BusinessObject(string name, IdentityUser author)
        {
            Name = name;
            CreatedOn = DateTime.Now;
            LastChanged = DateTime.Now;
            Author = author;
            Elements = new List<ApplicationElement>();
        }
    }
}
