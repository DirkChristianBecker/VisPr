namespace VisPrCore.Datamodel.Responses.User
{
    public class AuthenticationResponse
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
