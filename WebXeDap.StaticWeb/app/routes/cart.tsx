import { useMemo, useState } from "react";
import { useCartItems, useCheckoutCart } from "~/hooks/catalogs/use-cart";

export default function CartPage() {
	const { items, isLoading } = useCartItems();
	const checkout = useCheckoutCart();
	const [selected, setSelected] = useState<Set<number>>(new Set());
	const [message, setMessage] = useState<string | null>(null);

	function toggle(id: number) {
		const next = new Set(selected);
		if (next.has(id)) next.delete(id);
		else next.add(id);
		setSelected(next);
	}

	async function handleCheckout() {
		if (selected.size === 0) {
			setMessage("Select at least one item to pay.");
			return;
		}

		setMessage(null);
		const result = await checkout.mutateAsync({
			cartItemIDs: Array.from(selected),
			provider: 4,
			currencyCode: "VND",
		});

		setSelected(new Set());
		setMessage(
			result.providerPaymentUrl
				? "Payment created. Redirecting to provider..."
				: `Payment created for order #${result.orderID}.`,
		);

		if (result.providerPaymentUrl) {
			window.location.href = result.providerPaymentUrl;
		}
	}

	const total = useMemo(() => {
		if (!items) return 0;
		let s = 0;
		for (const it of items) {
			if (selected.has(it.id)) {
				s += it.product.price * it.quantity;
			}
		}
		return s;
	}, [items, selected]);

	if (isLoading) return <div className="p-6">Loading...</div>;

	return (
		<section className="p-6">
			<h1 className="text-2xl font-bold mb-4">Your cart</h1>
			{items && items.length > 0 ? (
				<div className="space-y-4">
					{items.map((it) => (
						<div
							key={it.id}
							className="flex items-center gap-4 p-4 border rounded"
						>
							<input
								type="checkbox"
								checked={selected.has(it.id)}
								onChange={() => toggle(it.id)}
							/>
							<img
								src={`https://picsum.photos/120/90?random=${it.product.id}`}
								className="w-24 h-16 object-cover"
							/>
							<div className="flex-1">
								<div className="font-semibold">{it.product.name}</div>
								<div className="text-sm text-muted-foreground">
									Qty: {it.quantity}
								</div>
							</div>
							<div className="font-semibold">
								{it.product.price.toLocaleString()} {it.product.currencySymbol}
							</div>
						</div>
					))}

					<div className="flex items-center justify-between mt-4">
						<div className="text-lg">Selected total:</div>
						<div className="text-lg font-bold">
							{total.toLocaleString()} VND
						</div>
					</div>

					{message && (
						<div className="text-sm text-muted-foreground">{message}</div>
					)}

					<div className="mt-4">
						<button
							className="px-4 py-2 bg-primary text-white rounded disabled:opacity-60"
							onClick={handleCheckout}
							disabled={checkout.isPending}
						>
							{checkout.isPending ? "Processing..." : "Proceed to pay"}
						</button>
					</div>
				</div>
			) : (
				<div className="text-muted-foreground">Your cart is empty.</div>
			)}
		</section>
	);
}
