import { useQuery } from "@tanstack/react-query";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";

export interface SimpleProductResponse {
	id: number;
	name: string;
	description?: string;
	price: number;
	currencySymbol: string;
	quantity: number;
	image?: {
		id: number;
		url: string;
		order: number;
	};
	createdAt: string;
	updatedAt?: string;
}

interface PagedProductResponse {
	data: SimpleProductResponse[];
	total: number;
	page: number;
	size: number;
}

export interface FilterQuery {
	keyword?: string;
	minPrice?: number;
	maxPrice?: number;
	categoryIDs?: number[];
	sortBy?: "name" | "price" | "createdAt";
	isAscending?: boolean;
	page?: number;
	size?: number;
}
function filterProduct(query: FilterQuery) {
	const searchParams = new URLSearchParams();
	if (query.keyword) searchParams.append("keyword", query.keyword);
	if (query.minPrice)
		searchParams.append("minPrice", query.minPrice.toString());
	if (query.maxPrice)
		searchParams.append("maxPrice", query.maxPrice.toString());
	if (query.categoryIDs && query.categoryIDs.length > 0)
		query.categoryIDs.forEach((id) =>
			searchParams.append("categoryIDs", id.toString()),
		);
	if (query.sortBy) searchParams.append("sortBy", query.sortBy);
	if (query.isAscending !== undefined)
		searchParams.append("isAscending", query.isAscending.toString());
	if (query.page) searchParams.append("page", query.page.toString());
	if (query.size) searchParams.append("size", query.size.toString());

	return client
		.get(ENDPOINTS.PRODUCTS, {
			searchParams: searchParams,
		})
		.json<PagedProductResponse>();
}

export function useProducts(query: FilterQuery) {
	const { data, isLoading } = useQuery({
		queryKey: ["products", query],
		queryFn: async () => await filterProduct(query),
	});

	const products = data?.data;
	const total = data?.total;
	const error = data ? undefined : "Failed to fetch products";

	return { products, total, isLoading, error };
}
