using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using VisPrCore.Datamodel.Requests.Runtime;
using VisPrCore.Datamodel.Responses.Runtime;
using VisPrCore.Datamodel.Responses;
using VisPrCore.Datamodel;
using VisPrRuntime.Controllers;
using VisPrRuntime.Services;
using VisPr_Runtime.Services.DesktopRecorder;

namespace VisPr_Runtime.Controllers
{
    [Route("[controller]/[action]")]
    public class DesktopRecorderController : ControllerBase
    {
        private ILogger<DesktopRecorderController> Logger;
        private DesktopRecorderService Recorder { get; set; }

        public DesktopRecorderController(ILogger<DesktopRecorderController> log, DesktopRecorderService recorder)
        {
            Logger = log;
            Recorder = recorder;
        }

        [HttpPut("{application_id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult> Start([FromRoute] int application_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            await Task.Run(() => Recorder.Start(application_id));
            return Ok(new MessageResponse("Recorder started"));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult> Stop()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            await Task.Run(() => Recorder.Stop());
            return Ok(new MessageResponse("Recorder stoppped"));
        }
    }
}
