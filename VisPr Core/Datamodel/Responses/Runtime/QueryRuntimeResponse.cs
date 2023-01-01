using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrCore.Datamodel.Responses.Runtime
{
    public class QueryRuntimeResponse
    {
        public List<UiElementDescriptor> Elements { get; set; }

        public QueryRuntimeResponse() 
        {
            Elements = new List<UiElementDescriptor>();
        }
    }
}
