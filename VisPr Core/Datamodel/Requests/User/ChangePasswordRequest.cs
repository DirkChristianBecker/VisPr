using System.ComponentModel.DataAnnotations;

namespace VisPrCore.Datamodel.Requests.User
{
    public class ChangePasswordRequest
    {
        [Required] public string? UserName { get; set; }
        [Required] public string? Password { get; set; }
        [Required] public string? NewPassword { get; set; }
    }
}
