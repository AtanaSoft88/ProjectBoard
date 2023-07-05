namespace ProjectBoard.Identity.Abstractions;

public class IdentityOptions
{
    public string? Authority { get; set; }
    public string? AuthorizationUrl { get; set; }
    public string? TokenUrl { get; set; }
    public string? ClientId { get; set; }
    public string? UserPoolId { get; set; }    
}
