using System.Text.Json;
using System.Text.Json.Serialization;
using log4net;

namespace ZwooBackend.Services;

/// <summary>
/// a service for verifying captcha challenges
/// </summary>
public interface ICaptchaService
{
    /// <summary>
    /// verify a captcha token
    /// </summary>
    /// <param name="token">the user challenge generated token</param>
    public Task<CaptchaResponse?> Verify(string token);
}

public class CaptchaResponse
{
    public CaptchaResponse(bool success, string timestamp, double score, string[]? errors)
    {
        Success = success;
        Timestamp = timestamp;
        Score = score;
        Errors = errors;
    }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public string Timestamp { get; set; }

    public double Score { get; set; }

    [JsonPropertyName("error-codes")]
    public string[]? Errors { get; set; }
}

public class CaptchaService : ICaptchaService
{
    private readonly string _secret = Globals.RecaptchaSideSecret;
    private ILog _logger = LogManager.GetLogger("EmailService");

    public async Task<CaptchaResponse?> Verify(string token)
    {
        try
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["secret"] = _secret;
            data["response"] = token;

            var client = new HttpClient();
            var res = await client.PostAsync("https://hcaptcha.com/siteverify", new FormUrlEncodedContent(data));
            var body = await res.Content.ReadAsStringAsync();

            CaptchaResponse? response = JsonSerializer.Deserialize<CaptchaResponse>(body);
            if (response != null)
            {
                // hcaptcha specific
                response.Score = response.Success ? 1 : 0;
            }
            return response;
        }
        catch (Exception ex)
        {
            _logger.Warn("cant verify captcha token", ex);
            return null;
        }
    }
}
