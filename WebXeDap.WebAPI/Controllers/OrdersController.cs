using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebXeDap.Application.Contracts;
using WebXeDap.Application.Contracts.Persistence;

namespace WebXeDap.WebAPI.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrdersController : ControllerBase
{
	private readonly IApplicationDbContext ctx;
	private readonly ICurrentUserService currentUser;

	public OrdersController(IApplicationDbContext ctx, ICurrentUserService currentUser)
	{
		this.ctx = ctx;
		this.currentUser = currentUser;
	}

	[HttpGet]
	public async Task<IActionResult> List()
	{
		var uidRes = currentUser.UserID;
		var uid = uidRes.Match(v => (int?)v, _ => null);
		if (uid == null)
			return Forbid();
		var orders = await ctx
			.Orders.Where(o => o.UserID == uid.Value)
			.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
			.Include(o => o.Payments)
			.OrderByDescending(o => o.OrderDate)
			.ToListAsync();

		var result = orders.Select(o => new
		{
			id = o.ID,
			total = o.TotalAmount,
			orderDate = o.OrderDate,
			status = o.Payments != null && o.Payments.Count > 0
				? o.Payments.First().Status.ToString()
				: null,
			items = o.OrderItems.Select(i => new
			{
				name = i.Product.Name,
				qty = i.Quantity,
				unitPrice = i.UnitPrice,
			}),
		});

		return Ok(result);
	}
}
