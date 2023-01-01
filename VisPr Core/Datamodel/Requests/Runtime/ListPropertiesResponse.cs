using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrCore.Datamodel.Requests.Runtime
{
    public class ListPropertiesResponse
    {
        public List<string> UiA2Properties { get; set; }
        public List<string> UiA3Properties { get; set; }
        public ListPropertiesResponse()
        {
            UiA2Properties = new List<string>();
            UiA3Properties = new List<string>();
        }
    }
}
