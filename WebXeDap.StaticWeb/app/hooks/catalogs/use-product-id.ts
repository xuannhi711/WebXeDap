import { useQuery } from "@tanstack/react-query";
import { HTTPError } from "ky";
import { ResultAsync } from "neverthrow";
import { useState } from "react";
import { match, P } from "ts-pattern";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";

type ProductError =
	| { type: "product_not_found"; message: string }
	| { type: "unknown_error"; message: string };

export interface DetailedProductResponse {
	id: number;
	name: string;
	description?: string;
	price: number;
	currencySymbol: string;
	quantity: number;
	images: {
		id: number;
		url: string;
		order: number;
	}[];
	category: {
		id: number;
		name: string;
	};
	createdAt: string;
	updatedAt?: string;
}

function fetchProduct(id: number) {
	return ResultAsync.fromPromise(
		client.get(`${ENDPOINTS.PRODUCTS}/${id}`).json<DetailedProductResponse>(),
		(error): ProductError =>
			match(error as HTTPError)
				.with({ response: { status: 404 } }, () => ({
					type: "product_not_found" as const,
					message: "Product not found",
				}))
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error occurred",
				})),
	);
}

export function useProductID(id: number) {
	const { data, isLoading } = useQuery({
		queryKey: ["product", id],
		queryFn: async () => await fetchProduct(id),
	});

	const product = data?.unwrapOr(undefined);
	const error = data?.isErr() ? data.error : undefined;

	return { product, isLoading, error };
}
