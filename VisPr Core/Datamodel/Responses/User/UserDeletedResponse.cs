using Microsoft.AspNetCore.Identity;

namespace VisPrCore.Datamodel.Responses.User
{
    public class UserDeletedResponse
    {
        public string Response { get; set; }

        public UserDeletedResponse(IdentityUser u)
        {
            Response = string.Format("User '{0}' deleted.", u.UserName);
        }
    }
}
