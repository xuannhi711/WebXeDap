using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Cart;
using WebXeDap.Application.Catalog;
using WebXeDap.Application.Catalog.Validators;
using WebXeDap.Application.Orders;

namespace WebXeDap.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<CatalogService>();
		services.AddScoped<CartService>();
		services.AddScoped<OrderService>();

		services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();

		return services;
	}
}
