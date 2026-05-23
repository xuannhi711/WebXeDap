import { Minus, Plus, Trash2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { Button } from "~/components/ui/button";
import { Input } from "~/components/ui/input";
import {
	useCartItems,
	useCheckoutCart,
	useDeleteCartItem,
	useUpdateCartItem,
} from "~/hooks/catalogs/use-cart";

export default function CartPage() {
	const { items, isLoading } = useCartItems();
	const checkout = useCheckoutCart();
	const updateCartItem = useUpdateCartItem();
	const deleteCartItem = useDeleteCartItem();
	const [selected, setSelected] = useState<Set<number>>(new Set());
	const [message, setMessage] = useState<string | null>(null);
	const [draftQuantities, setDraftQuantities] = useState<Record<number, string>>(
		{},
	);

	useEffect(() => {
		if (!items) return;

		setDraftQuantities((current) => {
			const next: Record<number, string> = {};
			for (const item of items) {
				next[item.id] = current[item.id] ?? String(item.quantity);
			}
			return next;
		});
	}, [items]);

	function toggle(id: number) {
		const next = new Set(selected);
		if (next.has(id)) next.delete(id);
		else next.add(id);
		setSelected(next);
	}

	async function commitQuantity(itemID: number, value: string) {
		const nextQuantity = parseInt(value, 10);
		const currentItem = items?.find((item) => item.id === itemID);

		if (!currentItem) return;

		if (!Number.isFinite(nextQuantity) || nextQuantity < 1) {
			setDraftQuantities((current) => ({
				...current,
				[itemID]: String(currentItem.quantity),
			}));
			return;
		}

		if (nextQuantity === currentItem.quantity) {
			setDraftQuantities((current) => ({
				...current,
				[itemID]: String(nextQuantity),
			}));
			return;
		}

		await updateCartItem.mutateAsync({ cartItemID: itemID, quantity: nextQuantity });
		setDraftQuantities((current) => ({
			...current,
			[itemID]: String(nextQuantity),
		}));
	}

	async function changeQuantity(itemID: number, delta: number) {
		const currentItem = items?.find((item) => item.id === itemID);
		if (!currentItem) return;
		await commitQuantity(itemID, String(currentItem.quantity + delta));
	}

	async function removeItem(itemID: number) {
		await deleteCartItem.mutateAsync(itemID);
		setSelected((current) => {
			const next = new Set(current);
			next.delete(itemID);
			return next;
		});
		setDraftQuantities((current) => {
			const next = { ...current };
			delete next[itemID];
			return next;
		});
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
							className="flex flex-wrap items-center gap-4 p-4 border rounded"
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
									{it.product.price.toLocaleString()} {it.product.currencySymbol}
								</div>
							</div>
							<div className="flex items-center gap-2">
								<Button
									variant="outline"
									size="icon-sm"
									onClick={() => void changeQuantity(it.id, -1)}
									disabled={updateCartItem.isPending || deleteCartItem.isPending}
									aria-label={`Decrease quantity for ${it.product.name}`}
								>
									<Minus />
								</Button>
								<Input
									type="number"
									min={1}
									className="w-20 text-center"
									value={draftQuantities[it.id] ?? String(it.quantity)}
									onChange={(e) =>
										setDraftQuantities((current) => ({
											...current,
											[it.id]: e.target.value,
										}))
									}
									onBlur={() =>
										void commitQuantity(it.id, draftQuantities[it.id] ?? String(it.quantity))
									}
									onKeyDown={(e) => {
										if (e.key === "Enter") {
											e.currentTarget.blur();
										}
									}}
								/>
								<Button
									variant="outline"
									size="icon-sm"
									onClick={() => void changeQuantity(it.id, 1)}
									disabled={updateCartItem.isPending || deleteCartItem.isPending}
									aria-label={`Increase quantity for ${it.product.name}`}
								>
									<Plus />
								</Button>
							</div>
							<div className="font-semibold min-w-28 text-right">
								{(it.product.price * it.quantity).toLocaleString()} {it.product.currencySymbol}
							</div>
							<Button
								variant="destructive"
								size="icon-sm"
								onClick={() => void removeItem(it.id)}
								disabled={updateCartItem.isPending || deleteCartItem.isPending}
								aria-label={`Remove ${it.product.name} from cart`}
							>
								<Trash2 />
							</Button>
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
