import { ShoppingCartIcon } from "lucide-react";
import { useEffect } from "react";
import { Link } from "react-router";
import { useCountCartItems } from "~/hooks/catalogs/use-cart";
import { useStore } from "~/store/store";

export function NavCart() {
	const { mutateAsync } = useCountCartItems();
	const count = useStore((state) => state.cartCount);

	useEffect(() => {
		mutateAsync();
	}, [mutateAsync]);

	return (
		<Link to="/cart" className="relative">
			<ShoppingCartIcon className="h-6 w-6" />
			<span className="absolute -top-2 -right-3 bg-foreground text-background rounded-full size-5 flex items-center justify-center text-xs font-bold">
				{count}
			</span>
		</Link>
	);
}
