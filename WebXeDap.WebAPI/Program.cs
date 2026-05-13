using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebXeDap.Application;
using WebXeDap.Application.Contracts;
using WebXeDap.Application.Contracts.Persistence;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Domain.Constants;
using WebXeDap.Infrastructure;
using WebXeDap.WebAPI.Options;
using WebXeDap.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var dbProvider = builder.Configuration["DB_PROVIDER"];
var useSqlite = string.Equals(dbProvider, "sqlite", StringComparison.OrdinalIgnoreCase);
var connectionString = useSqlite
	? builder.Configuration["SQLITE_CONNECTION_STRING"] ?? "Data Source=webxedap.db"
	: builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
	throw new InvalidOperationException("Database connection string is not configured.");
}

builder.Services.AddInfrastructure(connectionString, useSqlite);
builder.Services.AddApplication();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserService>();

var jwtOptions = new JwtOptions();
builder.Configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);
jwtOptions.Key ??= builder.Configuration["JWT_SECRET"];

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
	throw new InvalidOperationException("JWT secret is not configured.");
}

if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
{
	jwtOptions.Issuer = "WebXeDap";
}

if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
{
	jwtOptions.Audience = "WebXeDap.Client";
}

if (jwtOptions.ExpiresMinutes <= 0)
{
	jwtOptions.ExpiresMinutes = 60;
}

if (jwtOptions.RefreshTokenExpiresDays <= 0)
{
	jwtOptions.RefreshTokenExpiresDays = 7;
}

builder.Services.AddSingleton(Options.Create(jwtOptions));

builder
	.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = jwtOptions.Issuer,
			ValidateAudience = true,
			ValidAudience = jwtOptions.Audience,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
			RoleClaimType = ClaimTypes.Role,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.FromMinutes(1),
		};
	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy(ROLES.ADMIN, policy => policy.RequireRole(ROLES.ADMIN));
	options.AddPolicy(ROLES.STAFF, policy => policy.RequireRole(ROLES.STAFF));
	options.AddPolicy(ROLES.CUSTOMER, policy => policy.RequireRole(ROLES.CUSTOMER));
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
