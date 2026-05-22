import { ArrowRight } from "lucide-react";
import type { ComponentProps } from "react";
import { Link } from "react-router";
import type { SimpleProductResponse } from "~/hooks/catalogs/use-product";
import { cn } from "~/lib/utils";

type CardProductProps = {
	product: SimpleProductResponse;
} & ComponentProps<"div">;

export function CardProduct(props: CardProductProps) {
	const product = props.product;
	const isInStock = product.quantity > 0;
	const price = `${product.price.toLocaleString("en-US")} ${product.currencySymbol}`;

	return (
		<Link
			to={`/products/${product.id}`}
			className={cn(
				"grid gap-2 p-4 border border-input hover:border-ring",
				props.className,
			)}
		>
			<img
				src={`https://picsum.photos/500/500?random=${product.id}`}
				loading="lazy"
				alt={product.name}
				className="aspect-3/2 object-cover"
			/>
			<div className="flex flex-col gap-2">
				<h3 className="text-xl font-bold">{product.name}</h3>
				<span className="text-muted-foreground font-semibold">{price}</span>
				<span
					aria-busy={isInStock}
					className="font-light text-sm aria-busy:text-green-600 text-destructive"
				>
					{isInStock ? "In Stock" : "Out of Stock"}
				</span>
				<p className="ml-auto flex gap-1 items-center">
					Learn more <ArrowRight />
				</p>
			</div>
		</Link>
	);
}
