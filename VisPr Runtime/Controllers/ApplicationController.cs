using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using VisPrRuntime.Services;
using VisPrCore.Datamodel.Requests.Runtime;
using VisPrCore.Datamodel.Responses.Runtime;
using VisPrCore.Datamodel.Responses;
using VisPrCore.Datamodel;

namespace VisPrRuntime.Controllers
{
    [Route("[controller]/[action]")]
    public class ApplicationController : ControllerBase
    {
        private IQueryApplication Applications { get; set; }
        private System.Drawing.Color DefaultHighlightColor { get; set; }
        private int DefaultHightlightTimeout { get; set; }

        public ApplicationController(IQueryApplication applications, IConfiguration config)
        {
            Applications = applications;

            DefaultHighlightColor = config.ReadHighlightColor();
            DefaultHightlightTimeout = config.ReadHighlightTimeout();
        }

        /// <summary>
        /// Launch an application and initialize an automation object. Returns an application id 
        /// that identifies the launched application. 
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<LaunchApplicationResponse>> Launch([FromBody] LaunchApplicationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var r = new LaunchApplicationResponse();
            r.ApplicationId = await Task.Run(() => Applications.Launch(request.API, request.Application, request.Arguments));

            return r;
        }

        /// <summary>
        /// Terminate an application. If the timeout is set the application will be forcefully terminated
        /// after waiting for the given time.
        /// Returns true, if the application could be gracefully terminated. False otherwise.
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<MessageResponse>> Terminate([FromBody] TerminateApplicationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TimeSpan? timeout = request.TimeOut.HasValue ? TimeSpan.FromSeconds(request.TimeOut.Value) : null;
            var r = new MessageResponse(
                string.Format("Application gracefully terminated: {0}", 
                await Task.Run(() => Applications.Terminate(request.ApplicationId, timeout))));

            return r;
        }

        /// <summary>
        /// List top level UI-ELements for an application.
        /// 
        /// </summary>
        /// <param name="application_id"></param>
        /// <returns></returns>
        [HttpGet("{application_id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<QueryRuntimeResponse>> ListUIElements([FromRoute] int application_id)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var r = await Task.Run(() => Applications.ListApplicationModel(application_id));
            return r;
        }

        /// <summary>
        /// List the direct children of an UI-Element.
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<QueryRuntimeResponse>> ListChildren([FromBody] ListChildrenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            var r = await Task.Run(() => Applications.ListChildren(request.ApplicationId, request.SelectorType, request.Selectors));
            return r;
        }

        [HttpGet]
        public async Task<ActionResult<ListPropertiesResponse>> ListProperties()
        {
            var r = await Task.Run(() => Applications.ListProperties());
            return r;
        }


        /// <summary>
        /// Draw a highlight around the element with the given settings.
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<UiElementDescriptor>> DrawHighlight([FromBody] DrawHighlightRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var color = System.Drawing.Color.FromArgb(
                request.Red ?? DefaultHighlightColor.R, 
                request.Green ?? DefaultHighlightColor.G, 
                request.Blue ?? DefaultHighlightColor.B);

            var r = await Applications.DrawHighlight(
                request.ApplicationId, 
                request.ElementType, 
                request.Selectors,
                color, 
                TimeSpan.FromMilliseconds(request.Duration ?? DefaultHightlightTimeout));

            return r;
        }

        /// <summary>
        /// Waits until the element has a clickable point.
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<UiElementDescriptor>> WaitUntilClickable([FromBody] WaitForElementRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var r = await Applications.WaitUntilEnabled(
                    request.ApplicationId,
                    request.ElementType,
                    request.Selectors,
                    TimeSpan.FromSeconds(request.Duration ?? 2));

                return r;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Waits until the element is enabled.
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
                   Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<UiElementDescriptor>> WaitUntilEnabled([FromBody] WaitForElementRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var r = await Applications.WaitUntilEnabled(
                    request.ApplicationId,
                    request.ElementType,
                    request.Selectors,
                    TimeSpan.FromSeconds(request.Duration ?? 2));

                return r;
            } 
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
