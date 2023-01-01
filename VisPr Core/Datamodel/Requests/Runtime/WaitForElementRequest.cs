using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;

namespace VisPrCore.Datamodel.Requests.Runtime
{
    public class WaitForElementRequest
    {
        /// <summary>
        /// Application id as returned from Launch or Attach Application.
        /// 
        /// </summary>
        [Required] public int ApplicationId { get; set; }

        /// <summary>
        /// Which API to use.
        /// 
        /// </summary>
        [Required] public ElementType ElementType { get; set; }

        /// <summary>
        /// List of selectors to identify the UI-Element. The selectors are beeing And-Combined into Property Conditions.
        /// 
        /// </summary>
        [Required] public List<ElementSelector> Selectors { get; set; }

        /// <summary>
        /// Duration of the highlight in seconds.
        /// 
        /// </summary>
        public int? Duration { get; set; }

        public WaitForElementRequest()
        {
            Selectors = new List<ElementSelector>();
        }
    }
}
