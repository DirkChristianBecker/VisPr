using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using VisPrCore.Datamodel;
using VisPrCore.Datamodel.Responses.Runtime;
using VisPrCore.Datamodel.Requests.Runtime;
using VisPrCore.Datamodel.Responses;
using VisPrRuntime.Services;

namespace VisPrRuntime.Controllers
{
    [Route("[controller]/[action]")]
    public class RuntimeController : ControllerBase
    {
        private ILogger<RuntimeController> Logger;
        private IQueryDesktop Desktop { get; set; }

        public RuntimeController(ILogger<RuntimeController> log, IQueryDesktop desktop) 
        {
            Logger = log;
            Desktop = desktop;            
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<QueryRuntimeResponse>> ListUIElements([FromBody] QueryDesktopRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            var r = await Task.Run(() => Desktop.ListElements(request.ElementType));

            return r;
        }
    }
}
