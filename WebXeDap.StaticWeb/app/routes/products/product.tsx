import {
	ChevronsUpDownIcon,
	Heart,
	Minus,
	Plus,
	ShoppingCart,
	Truck,
} from "lucide-react";
import { useState } from "react";
import "swiper/css";
import "swiper/css/navigation";
import "swiper/css/pagination";
import { Navigation, Pagination } from "swiper/modules";
import { Swiper, SwiperSlide } from "swiper/react";
import { Button } from "~/components/ui/button";
import {
	Collapsible,
	CollapsibleContent,
	CollapsibleTrigger,
} from "~/components/ui/collapsible";
import { Input } from "~/components/ui/input";
import { useProductID } from "~/hooks/catalogs/use-product-id";
import { API_HOST } from "~/config/app";
import type { Route } from "./+types/product";
import { useAddToCart } from "~/hooks/catalogs/use-cart";

export default function ProductPage({ params }: Route.ComponentProps) {
	const { productId } = params;
	const [quantity, setQuantity] = useState(1);
	const { product, error, isLoading } = useProductID(parseInt(productId, 10));
	const addToCart = useAddToCart();

	return (
		<>
			{isLoading && <p>Loading...</p>}

			{error && <p>Error: {error.message}</p>}

			{product && (
				<div className="grid md:grid-cols-2 gap-10 px-35 py-10">
					<div>
						<Swiper
							slidesPerView={1}
							loop={true}
							pagination={{
								clickable: true,
							}}
							navigation={true}
							modules={[Pagination, Navigation]}
							className="max-w-md"
						>
							{product.images && product.images.length > 0
								? product.images.map((img) => (
										<SwiperSlide key={img.id}>
											<img
												src={
													img.url.startsWith("/")
														? `${API_HOST}${img.url}`
														: img.url
												}
												alt="Product"
												className="w-full"
											/>
										</SwiperSlide>
									))
								: Array.from({ length: 5 }).map((_, index) => (
										<SwiperSlide key={Math.random()}>
											<img
												src={`https://picsum.photos/500/500?random=${index + 1}`}
												alt="Product"
												className="w-full"
											/>
										</SwiperSlide>
									))}
						</Swiper>
					</div>

					<div>
						<h2 className="text-2xl font-bold">{product.name}</h2>

						<div className="mt-4 border border-border p-3 rounded flex items-center justify-between">
							<div className="flex items-center gap-2">
								<span className="h-3 w-3 rounded-full bg-emerald-500 inline-block" />
								<span className="font-medium">In stock online</span>
							</div>
							<div className="text-sm text-muted-foreground">
								Shipping & Pickup
							</div>
						</div>

						<div className="mt-4 border p-4 rounded">
							<div className="text-3xl font-extrabold">
								{product.price.toLocaleString("en-US")} {product.currencySymbol}
							</div>
							<div className="text-sm text-muted-foreground mt-1">
								or{" "}
								<span className="font-semibold">
									{Math.round(product.price / 4).toLocaleString("en-US")}{" "}
									{product.currencySymbol}
								</span>{" "}
								monthly
							</div>

							<div className="flex items-center gap-1 mt-4">
								<Button variant="outline" className="p-2">
									<Heart />
								</Button>

								<Button
									variant="outline"
									className="px-3"
									onClick={() => setQuantity((prev) => Math.max(prev - 1, 1))}
									aria-label="decrease"
								>
									<Minus />
								</Button>
								<Input
									className="w-16 text-center"
									value={quantity}
									onChange={(e) => setQuantity(parseInt(e.target.value) || 1)}
								/>
								<Button
									variant="outline"
									className="px-3"
									onClick={() => setQuantity((prev) => Math.min(prev + 1, 99))}
									aria-label="increase"
								>
									<Plus />
								</Button>

								<Button
									className="flex-1"
									data-slot="button"
									onClick={async () =>
										await addToCart.mutateAsync({
											productID: product.id,
											quantity,
										})
									}
								>
									<ShoppingCart className="mr-2" /> Add to cart
								</Button>
							</div>
						</div>

						<div className="mt-4">
							<Collapsible>
								<CollapsibleTrigger className="flex items-center justify-between w-full p-3 bg-muted rounded">
									<div className="flex items-center gap-2">
										<Truck /> Free Shipping & Collection
									</div>
									<ChevronsUpDownIcon />
								</CollapsibleTrigger>
								<CollapsibleContent className="p-3 text-sm text-muted-foreground border border-t-0 rounded-b">
									Free shipping on orders above $100. In-store collection
									available.
								</CollapsibleContent>
							</Collapsible>

							<Collapsible className="mt-3" defaultOpen={true}>
								<CollapsibleTrigger className="flex items-center justify-between w-full p-3 bg-muted rounded">
									<div className="font-medium">About This Product</div>
									<ChevronsUpDownIcon />
								</CollapsibleTrigger>
								<CollapsibleContent className="p-3 text-sm text-muted-foreground border border-t-0 rounded-b">
									{product.description}
								</CollapsibleContent>
							</Collapsible>
						</div>
					</div>
				</div>
			)}
		</>
	);
}
