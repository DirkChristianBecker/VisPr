using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;

namespace VisPrCore.Datamodel.Requests.Runtime
{
    public class ListChildrenRequest
    {
        [Required] public int ApplicationId { get; set; }
        [Required] public List<ElementSelector> Selectors { get; set; }
        [Required] public ElementType SelectorType { get; set; }
        public ListChildrenRequest() 
        {
            Selectors = new List<ElementSelector>();
        }
    }
}
