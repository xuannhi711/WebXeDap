namespace WebXeDap.WebAPI.Options;

public sealed class JwtOptions
{
	public const string SectionName = "Jwt";

	public string? Key { get; set; }
	public string Issuer { get; set; } = "WebXeDap";
	public string Audience { get; set; } = "WebXeDap.Client";
	public int ExpiresMinutes { get; set; } = 60;
	public int RefreshTokenExpiresDays { get; set; } = 7;
}
