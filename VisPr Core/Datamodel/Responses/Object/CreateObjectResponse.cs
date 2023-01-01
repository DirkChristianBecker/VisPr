using VisPrCore.Datamodel.Database.ApplicationModeller;

namespace VisPrCore.Datamodel.Responses.Object
{
    public class CreateObjectResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CreateObjectResponse(BusinessObject n)
        {
            Id = n.Id;
            Name = n.Name;
        }

    }
}
