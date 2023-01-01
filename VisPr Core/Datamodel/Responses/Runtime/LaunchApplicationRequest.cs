using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPrCore.Datamodel.Database.ApplicationModel.ApplicationModeller;

namespace VisPrCore.Datamodel.Responses.Runtime
{
    public class LaunchApplicationRequest
    {
        [Required] public ElementType API { get; set; }
        [Required] public string Application { get; set; }
        public string? Arguments { get; set; }

        public LaunchApplicationRequest() : this("")
        {
        }

        public LaunchApplicationRequest(string app)
        {
            Application = app;
        }
    }
}
