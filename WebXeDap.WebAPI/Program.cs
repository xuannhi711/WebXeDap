using WebXeDap.Application;
using WebXeDap.Domain.Constants;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddHttpContextAccessor();

builder
	.Services.AddAuthorizationBuilder()
	.AddPolicy(ROLES.ADMIN, policy => policy.RequireRole(ROLES.ADMIN))
	.AddPolicy(ROLES.STAFF, policy => policy.RequireRole(ROLES.STAFF))
	.AddPolicy(ROLES.CUSTOMER, policy => policy.RequireRole(ROLES.CUSTOMER));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
	options.AddPolicy(
		"frontend",
		policy =>
		{
			policy
				.WithOrigins("http://localhost:5173")
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials();
		}
	);
});

builder
	.Services.AddAuthentication()
	.AddGoogle(opts =>
	{
		opts.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")!;
		opts.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")!;
	});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseCors("frontend");
}

if (!app.Environment.IsEnvironment("Testing"))
{
	app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/api/auth").MapIdentityApi<User>();
app.MapControllers();

app.Run();

public partial class Program { }
