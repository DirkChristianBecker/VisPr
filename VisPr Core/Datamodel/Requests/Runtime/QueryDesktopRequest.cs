using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;

namespace VisPrCore.Datamodel.Requests.Runtime
{
    public class QueryDesktopRequest
    {
        /// <summary>
        /// (Optional) Allows to narrow down the search results.
        /// 
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// Underlying API to use.
        /// 
        /// </summary>
        public ElementType ElementType { get; set; }
    }
}
