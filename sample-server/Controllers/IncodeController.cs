using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TokenServer.Models;

namespace TokenServer.Controllers;

[ApiController]
[Route("/")]
public class IncodeController : ControllerBase
{

    private readonly ILogger<IncodeController> log;
    private readonly IncodeSettings incodeSettings;

    private readonly HttpClient httpClient;

    public IncodeController(IOptions<IncodeSettings> settings, ILogger<IncodeController> logger)
    {
        log = logger;
        this.incodeSettings = settings.Value;

        httpClient = new();
        httpClient.DefaultRequestHeaders.Add("x-api-key", incodeSettings.ApiKey);
        httpClient.DefaultRequestHeaders.Add("api-version", "1.0");
    }


    private async Task<OmniStartResponse> CreateIncodeSession(){
        var startUrl = $"{incodeSettings.ApiUrl}/omni/start";
        log.LogInformation("Calling /omni/start");
        var payload = new
        {
            configurationId = incodeSettings.ConfigurationId,
            countryCode = "ALL",
            language = "en-US"
        };

        var response = await httpClient.PostAsJsonAsync(startUrl, payload);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var omniStartResponse = JsonSerializer.Deserialize<OmniStartResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return omniStartResponse!;
    }

    private async Task<FetchOnboardingUrlResponse> GetOnboardingUrl(string token, string? redirectionUrl){
        var url = $"{incodeSettings.ApiUrl}/omni/onboarding-url?clientId={incodeSettings.ClientId }";
        if (redirectionUrl != null){
            url += $"&redirectionUrl=${redirectionUrl}";
        }

        log.LogInformation( $"Calling { url }" );

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-Incode-Hardware-Id", token);
        
        var response = await httpClient.SendAsync(request);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var fetchUrlResponse = JsonSerializer.Deserialize<FetchOnboardingUrlResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return fetchUrlResponse!;
    }

    [HttpGet]
    [Route("start")]
    public async Task<IActionResult> CreateSession()
    {
        var omniStartResponse = await CreateIncodeSession();

        var response = new {
            omniStartResponse.InterviewId,
            omniStartResponse.Token 
        };

        return new JsonResult(response);
    }

    [HttpGet]
    [Route("onboarding-url")]
    public async Task<IActionResult> CreateSessionWithRedirectUrl([FromQuery] string redirectionUrl)
    {
        var omniStartResponse = await CreateIncodeSession();
        var onboardingUrlResponse = await GetOnboardingUrl(omniStartResponse.Token!, redirectionUrl);

        var response =  new {
            omniStartResponse.InterviewId,
            omniStartResponse.Token,
            onboardingUrlResponse.Url
        };
        
        return new JsonResult(response);
    }

    [HttpPost]
    [Route("webhook")]
    public IActionResult WebhookAction([FromBody] WebhookPayload data){
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var response = new {
            timeStamp,
            data
        };

        Console.WriteLine(JsonSerializer.Serialize(response));
        return new JsonResult(response);
    }
}
