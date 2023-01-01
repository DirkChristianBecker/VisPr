using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VisPrCore.Datamodel.Requests.User;
using VisPrCore.Datamodel.Responses.User;
using VisPrCore.Datamodel.Responses;
using VisPrCore.Services;

namespace VisPrRuntime.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private UserManager<IdentityUser> UserManager;
        private JwtService JwtService;
        private ILogger<UserController> Logger;

        public UserController(UserManager<IdentityUser> userManager, ILogger<UserController> log, JwtService jwt)
        {
            UserManager = userManager;
            JwtService = jwt;
            Logger = log;
        }

        /// <summary>
        /// Call this method to obtain a bearer token before making requests to other methods. The token must be send as an 'Authorization'-Header in each request.
        /// A bearer token expires so make sure to check the expiration value, if a request fails.
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(new MessageResponse("Bad credentials"));
            }

            if(request.UserName == null) 
            {
                return BadRequest(new MessageResponse("Username is missing"));
            }

            if (request.Password == null)
            {
                return BadRequest(new MessageResponse("Password is missing"));
            }

            var user = await UserManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            var isPasswordValid = await UserManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            var roles = await UserManager.GetRolesAsync(user);
            var token = JwtService.CreateToken(user, roles);

            Logger.Log(LogLevel.Debug, "Token created for {0}", request.UserName);

            return Ok(token);
        }
    }
}
