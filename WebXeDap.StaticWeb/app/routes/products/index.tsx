import { useMemo, useState, useEffect } from "react";
import { Link, useLocation } from "react-router";
import { CardProduct } from "~/components/cards/card-product";
import { useProducts } from "~/hooks/catalogs/use-product";
import { useDebounce } from "~/hooks/use-debounce";
import { usePagedQuery } from "~/hooks/use-paged-query";
import { useCategories } from "~/hooks/catalogs/use-categories";

const PRODUCTS_PER_PAGE = 20;

export function meta() {
	return [{ title: "Wheelie | Products" }];
}

function renderCategoryTree(
	items: any[],
	selected: Set<number>,
	onToggle: (id: number) => void,
) {
	return (
		<ul className="pl-4">
			{items.map((c) => (
				<li key={c.id} className="mb-1">
					<label className="inline-flex items-center gap-2">
						<input
							type="checkbox"
							checked={selected.has(c.id)}
							onChange={() => onToggle(c.id)}
						/>
						<span>{c.name}</span>
					</label>
					{c.children &&
						c.children.length > 0 &&
						renderCategoryTree(c.children, selected, onToggle)}
				</li>
			))}
		</ul>
	);
}

export default function ProductsIndex() {
	const location = useLocation();
	const { page, size, setPaged } = usePagedQuery(1, PRODUCTS_PER_PAGE);
	const [keyword, setKeyword] = useState("");

	const params = useMemo(
		() => new URLSearchParams(location.search),
		[location.search],
	);
	const categoryIDs = useMemo(() => {
		const vals = params
			.getAll("categoryIDs")
			.map((v) => parseInt(v, 10))
			.filter((n) => Number.isFinite(n));
		return vals;
	}, [params]);

	const query = useMemo(
		() => ({ page, size, keyword, categoryIDs }),
		[page, size, keyword, categoryIDs],
	);
	const debouncedQuery = useDebounce(query, 800);

	const { products, total, isLoading } = useProducts(debouncedQuery);
	const { categories } = useCategories();

	const totalPages = Math.max(1, Math.ceil((total ?? 0) / PRODUCTS_PER_PAGE));

	const [selectedCategories, setSelectedCategories] = useState<Set<number>>(
		new Set(categoryIDs),
	);

	// keep selectedCategories in sync with URL params
	useEffect(() => setSelectedCategories(new Set(categoryIDs)), [categoryIDs]);

	function toggleCategory(id: number) {
		const next = new Set(selectedCategories);
		if (next.has(id)) next.delete(id);
		else next.add(id);

		const params = new URLSearchParams(location.search);
		params.delete("categoryIDs");
		// append each selected id as repeated pair
		for (const v of Array.from(next)) {
			params.append("categoryIDs", String(v));
		}
		// ensure page param preserved/reset as needed
		// navigate via setPaged to preserve behavior; setPaged will replace page and size
		// but we want to keep the new category params as well – so directly navigate
		const newSearch = params.toString();
		// use history replace via location
		window.history.replaceState(
			{},
			"",
			`${location.pathname}${newSearch ? `?${newSearch}` : ""}`,
		);
		setSelectedCategories(next);
		// reset page to 1
		setPaged(1);
	}

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
							setPaged(1);
						}}
						className="px-3 py-2 border rounded-md"
					/>
					<Link to="/products/new" className="text-sm text-primary">
						New
					</Link>
				</div>
			</header>

			<div className="flex gap-6">
				<aside className="w-64">
					<h3 className="font-semibold mb-2">Categories</h3>
					{categories && categories.length > 0 ? (
						renderCategoryTree(categories, selectedCategories, toggleCategory)
					) : (
						<div className="text-sm text-muted-foreground">No categories</div>
					)}
				</aside>

				<div className="flex-1">
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
									<button
										disabled={page <= 1}
										onClick={() => setPaged(Math.max(1, page - 1))}
										className="px-3 py-1 border rounded disabled:opacity-50"
									>
										Previous
									</button>
									<button
										disabled={page >= totalPages}
										onClick={() => setPaged(Math.min(totalPages, page + 1))}
										className="px-3 py-1 border rounded disabled:opacity-50"
									>
										Next
									</button>
								</div>
							</footer>
						</>
					)}
				</div>
			</div>
		</section>
	);
}
