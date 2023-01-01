using System.ComponentModel.DataAnnotations;

namespace VisPrCore.Datamodel.Requests.User
{
    public class CreateUserRequest
    {
        [Required] public string? UserName { get; set; }
        [Required] public string? Password { get; set; }
        [Required] public string? Email { get; set; }
        [Required] public string? Role { get; set; }
    }
}
