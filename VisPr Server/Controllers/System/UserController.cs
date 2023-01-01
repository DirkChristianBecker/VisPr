using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VisPrCore.Datamodel;
using VisPrCore.Datamodel.Requests.User;
using VisPrCore.Datamodel.Responses;
using VisPrCore.Datamodel.Responses.User;
using VisPrCore.Services;

namespace VisPrServer.Controllers.System
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

        /// <summary>
        /// Use this method to change a password. Users can only change their own password. 
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ChangePasswordResponse>> ChangePassword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new MessageResponse("Bad credentials"));
            }

            if (request.UserName == null)
            {
                return BadRequest(new MessageResponse("Username is missing"));
            }

            if (request.Password == null)
            {
                return BadRequest(new MessageResponse("Password is missing"));
            }

            if (request.NewPassword == null)
            {
                return BadRequest(new MessageResponse("New password is missing"));
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

            var r = await UserManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
            if (!r.Succeeded)
            {
                return BadRequest(r.Errors);
            }

            Logger.Log(LogLevel.Debug, "Password changed created for {0}", request.UserName);

            return Ok(new ChangePasswordResponse(user));
        }

        /// <summary>
        /// Create a new user. The requesting user must be Admin.
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = Names.AdminName)]
        public async Task<ActionResult<CreateUserRequest>> NewUser(CreateUserRequest user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(user.Password == null)
            {
                return BadRequest(new MessageResponse("Password is missing"));
            }

            if (user.Role == null)
            {
                return BadRequest(new MessageResponse("Role is missing"));
            }

            var new_user = new IdentityUser() { UserName = user.UserName, Email = user.Email };

            var result = await UserManager.CreateAsync(new_user, user.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var role_result = await UserManager.AddToRoleAsync(new_user, user.Role);
            if (!role_result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            user.Password = null;

            Logger.Log(LogLevel.Debug, "New user '{0}' created.", new_user.UserName);

            return Created("", user);
        }

        /// <summary>
        /// Delete a user. The requesting user must be Admin.
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = Names.AdminName)]
        public async Task<ActionResult<UserDeletedResponse>> RemoveUser(DeleteUserRequest user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user.UserName == null)
            {
                return BadRequest(new MessageResponse("User name cannot be empty."));
            }

            var u = UserManager.Users.FirstOrDefault(x => x.UserName == user.UserName);
            if (u == null)
            {
                return BadRequest(new MessageResponse($"Could not find user '{user.UserName}'"));
            }

            var r = await UserManager.DeleteAsync(u);
            if (!r.Succeeded)
            {
                return BadRequest(r.Errors);
            }

            Logger.Log(LogLevel.Debug, "User '{0}' deleted.", u.UserName);

            return Ok(new UserDeletedResponse(u));
        }
    }
}
