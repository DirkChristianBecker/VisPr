using System.ComponentModel.DataAnnotations;

namespace VisPrCore.Datamodel.Requests.Object
{
    public class CreateObjectRequest
    {
        [Required] public string Name { get; set; }
        public string Description { get; set; }

        public CreateObjectRequest() 
        {
            Name = "";
            Description= "";
        }
    }
}
