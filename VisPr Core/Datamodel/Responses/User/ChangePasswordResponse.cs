using Microsoft.AspNetCore.Identity;

namespace VisPrCore.Datamodel.Responses.User
{
    public class ChangePasswordResponse
    {
        public string Response { get; set; }

        public ChangePasswordResponse(IdentityUser u)
        {
            Response = string.Format("Password changed for '{0}'", u.UserName);
        }
    }
}
