using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebXeDap.Domain.Models;
using WebXeDap.WebAPI.Options;

namespace WebXeDap.WebAPI.Services;

public sealed class TokenService
{
	private readonly JwtOptions _jwtOptions;
	private readonly SigningCredentials _signingCredentials;

	public TokenService(IOptions<JwtOptions> jwtOptions)
	{
		_jwtOptions = jwtOptions.Value;
		if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
		{
			throw new InvalidOperationException("JWT secret is not configured.");
		}

		_signingCredentials = new SigningCredentials(
			new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
			SecurityAlgorithms.HmacSha256
		);
	}

	public TokensResult CreateTokenPair(User user, IEnumerable<string> roles)
	{
		var accessToken = CreateAccessToken(user, roles);
		var refreshToken = CreateRefreshToken();
		return new TokensResult(
			accessToken,
			refreshToken,
			DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes),
			DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenExpiresDays)
		);
	}

	private string CreateAccessToken(User user, IEnumerable<string> roles)
	{
		var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes);
		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		};
		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var descriptor = new SecurityTokenDescriptor
		{
			Issuer = _jwtOptions.Issuer,
			Audience = _jwtOptions.Audience,
			Expires = expiresAtUtc,
			Subject = new ClaimsIdentity(claims),
			SigningCredentials = _signingCredentials,
		};

		return new JsonWebTokenHandler().CreateToken(descriptor);
	}

	private static string CreateRefreshToken()
	{
		var bytes = RandomNumberGenerator.GetBytes(64);
		return Base64UrlEncoder.Encode(bytes);
	}
}

public sealed record TokensResult(
	string AccessToken,
	string RefreshToken,
	DateTime AccessTokenExpiresAtUtc,
	DateTime RefreshTokenExpiresAtUtc
);
