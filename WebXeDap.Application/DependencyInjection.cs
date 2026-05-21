using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Cart;
using WebXeDap.Application.Features.Cart.Mapper;
using WebXeDap.Application.Features.Cart.Validators;
using WebXeDap.Application.Features.Catalog;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Application.Features.Catalog.Validators;
using WebXeDap.Application.Features.Sales;
using WebXeDap.Application.Features.Sales.Mapper;
using WebXeDap.Application.Features.Sales.Validators;
using WebXeDap.Application.Features.Statistics;

namespace WebXeDap.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<ProductMapper>();
		services.AddScoped<CategoryMapper>();
		services.AddScoped<ProductImageMapper>();
		services.AddScoped<CartMapper>();
		services.AddScoped<SaleCampaignMapper>();

		services.AddScoped<ICategoryService, CategoryService>();
		services.AddScoped<IProductService, ProductService>();
		services.AddScoped<ICartService, CartService>();
		services.AddScoped<ISaleCampaignService, SaleCampaignService>();
		services.AddScoped<IStatisticsService, StatisticsService>();

		services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>();

		services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateProductValidator>();

		services.AddValidatorsFromAssemblyContaining<AddCartItemValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateCartItemValidator>();
		services.AddValidatorsFromAssemblyContaining<CreateSaleCampaignValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateSaleCampaignValidator>();

		return services;
	}
}
