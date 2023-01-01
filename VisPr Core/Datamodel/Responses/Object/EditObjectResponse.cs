using VisPrCore.Datamodel.Database.ApplicationModeller;

namespace VisPrCore.Datamodel.Responses.Object
{
    public class EditObjectResponse
    {
        public BusinessObject Object { get; set; }
        public EditObjectResponse(BusinessObject ob) 
        {
            Object = ob;
        }
    }
}
