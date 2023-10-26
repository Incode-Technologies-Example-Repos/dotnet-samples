namespace TokenServer.Models;

public record OmniStartResponse
{
    public string? InterviewId { get; init; }
    public string? Token { get; init; }
    public string? InterviewCode { get; init; }
    public string? FlowType { get; init; }
    public int IdCaptureTimeout { get; init; }
    public int SelfieCaptureTimeout { get; init; }
    public int IdCaptureRetries { get; init; }
    public int SelfieCaptureRetries { get; init; }
    public int CurpValidationRetries { get; init; }
    public string? ClientId { get; init; }
    public string? Env { get; init; }
    public bool ExistingSession { get; init; }
    public string? EndScreenTitle { get; init; }
    public string? EndScreenText { get; init; }
    public bool OptinEnabled { get; init; }
    public string? OptinCompanyName { get; init; }
}

public record FetchOnboardingUrlResponse(string Url);

public record WebhookPayload {
    public string? InterviewId { get; init; }
    public string? OnboardingStatus{ get; init; }
    public string? ClientId{ get; init; }
    public string? FlowId{ get; init; }
}