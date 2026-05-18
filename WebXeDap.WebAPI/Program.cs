using Microsoft.AspNetCore.Identity;
using WebXeDap.Application;
using WebXeDap.Application.Contracts;
using WebXeDap.Domain.Constants;
using WebXeDap.Domain.Models;
using WebXeDap.Infrastructure;
using WebXeDap.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IEmailSender<User>, NoOpEmailSender>();

builder
	.Services.AddAuthorizationBuilder()
	.AddPolicy(ROLES.ADMIN, policy => policy.RequireRole(ROLES.ADMIN))
	.AddPolicy(ROLES.STAFF, policy => policy.RequireRole(ROLES.STAFF))
	.AddPolicy(ROLES.CUSTOMER, policy => policy.RequireRole(ROLES.CUSTOMER));

builder.Services.AddControllers();

// required for MapIdentityApi
builder.Services.AddAuthentication()
	.AddCookie(IdentityConstants.BearerScheme);
	
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
app.MapGroup("/api/auth").MapIdentityApi<User>();
app.MapControllers();

app.Run();
