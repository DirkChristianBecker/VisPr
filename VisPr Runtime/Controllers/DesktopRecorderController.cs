using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisPr_Runtime.Services;
using VisPrCore.Datamodel;
using VisPrCore.Datamodel.Responses;

namespace VisPrRuntime.Controllers
{
    [Route("[controller]/[action]")]
    public class DesktopRecorderController : ControllerBase
    {
        private ILogger<DesktopRecorderController> Logger;
        private IRuntimeLocation RuntimeLocation { get; set; }
        private IDesktopRecorderService RecorderService { get; set; }

        public DesktopRecorderController(
            ILogger<DesktopRecorderController> log,
            IDesktopRecorderService service,
            IRuntimeLocation location)
        {
            Logger = log;
            RuntimeLocation = location;
            RecorderService = service;
        }

        /// <summary>
        /// Launch the recorder application.
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult> Launch()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            await RecorderService.LaunchRecorderApplication();
            
            return Ok(new MessageResponse("Recorder started"));
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult> Terminate()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            await RecorderService.TerminateRecorderApplication();

            return Ok(new MessageResponse("Recorder terminated"));
        }


        /// <summary>
        /// Launch an application an start recording.
        /// 
        /// </summary>
        /// <param name="application_id">Id of the application that has been started.</param>
        /// <returns></returns>
        [HttpPut("{application_id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult> Start([FromRoute] int? application_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            await RecorderService.LaunchRecorderApplication();
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

            await RecorderService.TerminateRecorderApplication();
            return Ok(new MessageResponse("Recorder stoppped"));
        }
    }
}
