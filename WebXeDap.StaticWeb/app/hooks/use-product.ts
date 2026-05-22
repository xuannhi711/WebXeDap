import { HTTPError } from "ky";
import { ResultAsync } from "neverthrow";
import { match, P } from "ts-pattern";
import { client } from "~/lib/httpClient";

export interface ProductResponse {
	id: number;
	name: string;
	price: number;
	quantity: number;
	// add other fields as needed
}

type ProductError = { type: "unknown_error"; message: string };

function fetchProduct(id: number) {
	return ResultAsync.fromPromise(
		client.get(`/api/products/${id}`).json<ProductResponse>(),
		(error): ProductError =>
			match(error)
				.with(P.instanceOf(HTTPError), (err) => ({
					type: "unknown_error" as const,
					message: err.message,
				}))
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error",
				})),
	);
}

export function useProduct() {
	async function getById(id: number) {
		return await fetchProduct(id);
	}

	return { getById };
}
