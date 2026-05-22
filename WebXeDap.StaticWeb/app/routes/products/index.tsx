import { useMemo, useState } from "react";
import { Link } from "react-router";
import { CardProduct } from "~/components/cards/card-product";
import { useProducts } from "~/hooks/catalogs/use-product";
import { useDebounce } from "~/hooks/use-debounce";

const PRODUCTS_PER_PAGE = 20;

export function meta() {
	return [{ title: "Wheelie | Products" }];
}

export default function ProductsIndex() {
	const [page, setPage] = useState(1);
	const [keyword, setKeyword] = useState("");

	const query = useMemo(
		() => ({ page, size: PRODUCTS_PER_PAGE, keyword }),
		[page, keyword],
	);
	const debouncedQuery = useDebounce(query, 800);

	const { products, total, isLoading } = useProducts(debouncedQuery);

	const totalPages = Math.max(1, Math.ceil((total ?? 0) / PRODUCTS_PER_PAGE));

	return (
		<section className="px-15 py-10">
			<header className="flex items-center justify-between mb-6">
				<h1 className="text-2xl font-bold">Products</h1>
				<div className="flex items-center gap-2">
					<input
						placeholder="Search products..."
						value={keyword}
						onChange={(e) => {
							setKeyword(e.target.value);
							setPage(1);
						}}
						className="px-3 py-2 border rounded-md"
					/>
					<Link to="/products/new" className="text-sm text-primary">
						New
					</Link>
				</div>
			</header>

			{isLoading && <div>Loading...</div>}

			{!isLoading && (
				<>
					<div className="grid grid-cols-4 gap-6">
						{products && products.length > 0 ? (
							products.map((p) => <CardProduct key={p.id} product={p} />)
						) : (
							<div className="col-span-4 text-center text-muted-foreground">
								No products found.
							</div>
						)}
					</div>

					<footer className="flex items-center justify-between mt-6">
						<div>
							<span className="text-sm text-muted-foreground">
								Page {page} of {totalPages}
							</span>
						</div>
						<div className="flex gap-2">
							{/* <button
                                disabled={page <= 1}
                                onClick={() => setPage((s) => Math.max(1, s - 1))}
                                className="px-3 py-1 border rounded disabled:opacity-50"
                            >
                                Previous
                            </button>
                            <button
                                disabled={page >= totalPages}
                                onClick={() => setPage((s) => Math.min(totalPages, s + 1))}
                                className="px-3 py-1 border rounded disabled:opacity-50"
                            >
                                Next
                            </button> */}
						</div>
					</footer>
				</>
			)}
		</section>
	);
}
