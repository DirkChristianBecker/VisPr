using System.ComponentModel.DataAnnotations;

namespace VisPrCore.Datamodel.Requests.User
{
    public class DeleteUserRequest
    {
        [Required] public string? UserName { get; set; }
    }
}
