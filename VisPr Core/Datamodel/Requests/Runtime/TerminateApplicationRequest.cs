using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrCore.Datamodel.Requests.Runtime
{
    public class TerminateApplicationRequest
    {
        /// <summary>
        /// Application id as obtained by Launch or Attach.
        /// 
        /// </summary>
        [Required] public int ApplicationId { get; set; }

        /// <summary>
        /// Timeout in seconds.
        /// 
        /// </summary>
        public int? TimeOut { get; set; }
    }
}
