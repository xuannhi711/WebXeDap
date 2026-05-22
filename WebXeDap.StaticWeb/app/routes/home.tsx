import { Store, Undo2, Wallet } from "lucide-react";
import { Link } from "react-router";
import { CardProduct } from "~/components/cards/card-product";
import { buttonVariants } from "~/components/ui/button";
import { useProducts } from "~/hooks/catalogs/use-product";

export function meta() {
	return [{ title: "Wheelie | Home" }];
}

const DEFAULT_BTN_CLASSES = buttonVariants({
	variant: "default",
	size: "lg",
});

export default function Home() {
	return (
		<>
			<Hero />
			<Ads />
			<HotDeals />
		</>
	);
}

function Hero() {
	return (
		<section className="block-screen relative">
			<div className="absolute top-2/6 left-30 text-7xl font-bold text-white z-10">
				Spring Refresh
				<span className="block text-4xl italic font-light">
					New Gear Collection
				</span>
				<Link to="/products/onsale" className={DEFAULT_BTN_CLASSES}>
					Shop Now
				</Link>
			</div>
			<div className="bg-black/30 backdrop-brightness-80 size-full absolute"></div>
			<img
				src="/landing/ban.jpg"
				alt="Banner"
				className="size-full object-cover"
			/>
		</section>
	);
}

function Ads() {
	return (
		<section className="relative grid grid-cols-3 bg-muted px-20">
			<div className="p-4 flex flex-row items-center gap-2">
				<Wallet />
				<div>
					<h3 className="text-lg font-bold">As Low As 0% APR Financing</h3>
					<p className="text-muted-foreground">From Affirm</p>
				</div>
			</div>
			<div className="p-4 flex flex-row items-center gap-2">
				<Store />
				<div>
					<h3 className="text-lg font-bold">Free Shipping on Gear Orders</h3>
					<p className="text-muted-foreground">Above $100 or pickup at store</p>
				</div>
			</div>
			<div className="p-4 flex flex-row items-center gap-2">
				<Undo2 />
				<div>
					<h3 className="text-lg font-bold">Free Returns & Exchanges</h3>
					<p className="text-muted-foreground">Conditions apply</p>
				</div>
			</div>
		</section>
	);
}

function HotDeals() {
	const { products, isLoading } = useProducts({ page: 1, size: 6 });

	if (isLoading) return <div>Loading...</div>;

	return (
		<section className="px-15 py-10">
			<h2 className="text-2xl font-bold mb-4">Hot Deals</h2>
			<div className="grid grid-cols-3 gap-10">
				{products?.map((product) => (
					<CardProduct key={product.id} product={product} />
				))}
			</div>
		</section>
	);
}
