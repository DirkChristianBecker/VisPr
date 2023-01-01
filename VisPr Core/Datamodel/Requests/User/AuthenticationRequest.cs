using System.ComponentModel.DataAnnotations;

namespace VisPrCore.Datamodel.Requests.User
{
    public class AuthenticationRequest
    {
        [Required] public string? UserName { get; set; }
        [Required] public string? Password { get; set; }
    }
}
