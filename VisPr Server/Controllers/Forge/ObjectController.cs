using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using VisPrCore.Datamodel;
using VisPrCore.Datamodel.Database;
using VisPrCore.Datamodel.Responses.Object;
using VisPrCore.Datamodel.Database.ApplicationModeller;
using VisPrCore.Datamodel.Requests.Object;
using VisPrCore.Datamodel.Responses;
using System.Security.Claims;

namespace VisPrServer.Controllers.Forge
{
    [Route("[controller]/[action]")]
    public class ObjectController : ControllerBase
    {
        private ILogger<ObjectController> Logger;
        private IConfiguration Configuration;

        public ObjectController(ILogger<ObjectController> log, IConfiguration configuration)
        {
            Logger = log;
            Configuration = configuration;
        }

        protected string? GetUserFromHttpContext()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var user = identity?.FindFirst(ClaimTypes.Name)?.Value;
                return user;
            }

            return null;
        }

        /// <summary>
        /// Get a list of all objects this VisPr² instance contains. If the filter is set, the results will
        /// be filtered using the name column.
        ///  
        /// The requesting user must be either admin or developer.
        /// </summary>
        /// <returns>A list of business objects with name, description and id.</returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<ListObjectsResponse>> ListObjects(string? filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var r = await Task.Run(() =>
            {
                using (var db = new VisPrDbContext(Configuration))
                {
                    if (filter == null)
                    {                    
                        return db.BusinessObjects.Select(
                            x => new ListObjectObject
                            {
                                Name = x.Name,
                                Description = x.Description,
                                Id = x.Id
                            }).ToList();
                    }
                    else
                    {
                        var r = db.BusinessObjects?.Where(x => x.Name.StartsWith(filter))?.Select(
                            x => new ListObjectObject
                            {
                                Name = x.Name,
                                Description = x.Description,
                                Id = x.Id
                            })?.ToList();

                        return r == null ? new List<ListObjectObject>() : r;
                    }
                }
            });

            return Ok(new ListObjectsResponse(r));
        }

        /// <summary>
        /// Create a new object. The requesting user must be either admin or developer.
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The name and the id of the newly created object.</returns>
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<CreateObjectResponse>> CreateObject([FromBody] CreateObjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var check = await Task.Run(() =>
            {
                using (var db = new VisPrDbContext(Configuration))
                {
                    return db.BusinessObjects.Where(x => x.Name == request.Name).FirstOrDefault();
                }
            });
            if(check != null)
            {
                return BadRequest(new MessageResponse($"A business object with the name '{request.Name}' already exists."));
            }

            var new_object = new BusinessObject();
            new_object.Name = request.Name;
            new_object.Description = request.Description;
            new_object.CreatedOn = DateTime.UtcNow;
            new_object.LastChanged = DateTime.UtcNow;

            var user = GetUserFromHttpContext();
            if(user == null)
            {
                return BadRequest(new MessageResponse($"A user name is missing in the request token."));
            }

            using(var db = new VisPrDbContext(Configuration))
            {
                var u = db.Users.FirstOrDefault(x => x.UserName == user);
                if(u == null) 
                {
                    return BadRequest(new MessageResponse($"A user with the name '{user}' does not exist."));
                }

                new_object.Author = u;
                db.BusinessObjects.Add(new_object);
                await db.SaveChangesAsync();
            }

            return Ok(new CreateObjectResponse(new_object));
        }

        /// <summary>
        /// Delete a object. The requesting user must be either in the Admin or a Developer. 
        /// </summary>
        /// <param name="request">The id of the object to delete.</param>
        /// <returns>A success or an error message.</returns>
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = Names.AdminName + ", " + Names.DevName)]
        public async Task<ActionResult<MessageResponse>> DeleteObject(ObjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using(var db = new VisPrDbContext(Configuration))
            {
                var check = db.BusinessObjects.FirstOrDefault(x => x.Id == request.Id);
                if(check == null)
                {
                    return NotFound(new MessageResponse($"A business object with the id '{request.Id}' does not exist."));
                }

                db.Remove(check);
                await db.SaveChangesAsync();

                return Ok(new MessageResponse($"The business object '{check.Name}' has been deleted."));
            }
        }

        /// <summary>
        /// Request a complete business object to start editing. The requesting user must be either in the Admin or a Developer.
        /// 
        /// </summary>
        /// <param name="request">The id of the object to edit.</param>
        /// <returns>A complete business object</returns>
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = Names.AdminName + ", " + Names.DevName)]
        public ActionResult<EditObjectResponse> EditObject(ObjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var db = new VisPrDbContext(Configuration))
            {
                var check = db.BusinessObjects.FirstOrDefault(x => x.Id == request.Id);
                if (check == null)
                {
                    return NotFound(new MessageResponse($"A business object with the id '{request.Id}' does not exist."));
                }

                return Ok(new EditObjectResponse(check));
            }
        }
    }
}
