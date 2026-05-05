using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Catalog;
using WebXeDap.Application.Features.Catalog.Validators;

namespace WebXeDap.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<ICategoryService, CategoryService>();

		services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>();
		services.AddValidatorsFromAssemblyContaining<DeleteCategoryValidator>();

		return services;
	}
}
