using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;

namespace VisPrCore.Datamodel.Requests.Runtime
{
    public class DrawHighlightRequest
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
        /// A value between 0 and 255 which determines how red the highlight will be.
        /// 
        /// </summary>
        public int? Red { get; set; }

        /// <summary>
        /// A value between 0 and 255 which determines how green the highlight will be.
        /// 
        /// </summary>
        public int? Green { get; set; }

        /// <summary>
        /// A value between 0 and 255 which determines how blue the highlight will be.
        /// 
        /// </summary>
        public int? Blue { get; set; }

        /// <summary>
        /// Duration of the highlight in seconds.
        /// 
        /// </summary>
        public int? Duration { get; set; }

        public DrawHighlightRequest() 
        {
            Selectors = new List<ElementSelector>();
        }
    }
}
