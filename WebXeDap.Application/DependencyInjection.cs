using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WebXeDap.Application.Contracts.Services;
using WebXeDap.Application.Features.Cart;
using WebXeDap.Application.Features.Cart.Mapper;
using WebXeDap.Application.Features.Cart.Validators;
using WebXeDap.Application.Features.Catalog;
using WebXeDap.Application.Features.Catalog.Mapper;
using WebXeDap.Application.Features.Catalog.Validators;

namespace WebXeDap.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<ProductMapper>();
		services.AddScoped<CategoryMapper>();
		services.AddScoped<ProductImageMapper>();
		services.AddScoped<CartMapper>();

		services.AddScoped<ICategoryService, CategoryService>();
		services.AddScoped<IProductService, ProductService>();
		services.AddScoped<ICartService, CartService>();

		services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>();
		services.AddValidatorsFromAssemblyContaining<DeleteCategoryValidator>();

		services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateProductValidator>();
		services.AddValidatorsFromAssemblyContaining<DeleteProductValidator>();
		services.AddValidatorsFromAssemblyContaining<AddCartItemValidator>();
		services.AddValidatorsFromAssemblyContaining<UpdateCartItemValidator>();
		services.AddValidatorsFromAssemblyContaining<DeleteCartItemValidator>();

		return services;
	}
}
