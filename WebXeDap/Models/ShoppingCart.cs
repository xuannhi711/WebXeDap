using System.Collections.Generic;
using System.Linq;

namespace WebXeDap.Models
{
	public class ShoppingCart
	{
		public List<Giohang> Items { get; set; } = new List<Giohang>();

		public void AddItem(Giohang item)
		{
			var existingItem = Items.FirstOrDefault(i => i.MaSP == item.MaSP);
			if (existingItem != null)
			{
				existingItem.SoLuong += item.SoLuong;
			}
			else
			{
				Items.Add(item);
			}
		}

		public void RemoveItem(string maSP)
		{
			Items.RemoveAll(i => i.MaSP == maSP);
		}

		public decimal GetTotalPrice()
		{
			return Items.Sum(i => i.DonGia * i.SoLuong);
		}
	}
}
