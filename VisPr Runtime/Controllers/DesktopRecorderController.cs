using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisPrRuntime.Services;
using VisPrCore.Datamodel;
using VisPrCore.Datamodel.Responses;
using VisPrCore.Datamodel.Requests.Runtime.DesktopRecorder;

namespace VisPrRuntime.Controllers
{
    [Route("[controller]/[action]")]
    public class DesktopRecorderController : ControllerBase
    {
        private ILogger<DesktopRecorderController> Logger;
        private IRuntimeLocation RuntimeLocation { get; set; }
        private IQueryApplication Applications { get; set; }

        public DesktopRecorderController(
            ILogger<DesktopRecorderController> log,
            IRuntimeLocation location,
            IQueryApplication apps)
        {
            Logger = log;
            RuntimeLocation = location;
            Applications = apps;
        }

        /// <summary>
        /// Launch recorder application.
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
            
            return Ok(new MessageResponse("Recorder started"));
        }

        /// <summary>
        /// Terminate recorder application.
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult> Terminate()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            return Ok(new MessageResponse("Recorder terminated"));
        }


        /// <summary>
        /// Start recording. 
        /// 
        /// </summary>
        /// <param name="request">Path to the application to start and record.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public ActionResult Start([FromBody] StartRecordingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            if (request.ApplicationPath == null)
            {
                return BadRequest(new MessageResponse("Application path cannot be empty."));
            }

            return Ok();
        }

        /// <summary>
        /// Stop recording an application.
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public ActionResult Stop()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            return Ok();
        }
    }
}
