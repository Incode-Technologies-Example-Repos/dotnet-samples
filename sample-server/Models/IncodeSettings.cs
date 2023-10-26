namespace TokenServer.Models;
public record IncodeSettings{
    public required string ApiKey { get; init; }
    public required string ApiUrl { get; init; }
    public required string ConfigurationId { get; init; }
    public required string ClientId { get; init; }
}
