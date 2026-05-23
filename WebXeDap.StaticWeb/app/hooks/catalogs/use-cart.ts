import { useMutation, useQuery } from "@tanstack/react-query";
import type { HTTPError } from "ky";
import { ResultAsync } from "neverthrow";
import { match } from "ts-pattern";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";
import { useStore } from "~/store/store";

export interface CartProductResponse {
	id: number;
	name: string;
	description?: string;
	price: number;
	currencySymbol: string;
	quantity: number;
}

export interface CartItemResponse {
	id: number;
	quantity: number;
	product: CartProductResponse;
}

export interface CheckoutPaymentPayload {
	cartItemIDs: number[];
	provider: number;
	currencyCode?: string;
}

export interface CheckoutPaymentResponse {
	id: number;
	orderID: number;
	provider: number;
	status: number;
	amount: number;
	currencyCode: string;
	referenceCode: string;
	providerTransactionID?: string | null;
	providerPaymentUrl?: string | null;
	failureReason?: string | null;
	completedAt?: string | null;
}

export function useAddToCart() {
	const { mutateAsync } = useCountCartItems();
	const mutation = useMutation({
		mutationFn: async (payload: AddToCartPayload) => {
			const res = await addToCart(payload);
			if (res.isOk()) {
				await mutateAsync();
			}
			return res;
		},
	});
	return mutation;
}

export function useCountCartItems() {
	const { setCartCount } = useStore((state) => state);
	const mutation = useMutation({
		mutationFn: async () => {
			const res = await countCartItems();
			if (res.isOk()) {
				setCartCount(res.value);
			}
			return res;
		},
	});

	return mutation;
}

export function useCartItems() {
	const { data, isLoading } = useQuery({
		queryKey: ["cartItems"],
		queryFn: async () => await listCartItems(),
	});

	const items = data;
	const error = data ? undefined : "Failed to fetch cart items";

	return { items, isLoading, error };
}

export function useCheckoutCart() {
	return useMutation({
		mutationFn: async (payload: CheckoutPaymentPayload) =>
			await checkoutCart(payload),
	});
}

type CartError = { type: "not_authenticated" } | { type: "unknown_error" };

async function countCartItems() {
	return ResultAsync.fromPromise(
		client.get(`${ENDPOINTS.CART}/count`).json<number>(),

		(error): CartError =>
			match(error as HTTPError)
				.with({ response: { status: 404 } }, () => ({
					type: "not_authenticated" as const,
				}))
				.otherwise(() => ({ type: "unknown_error" as const })),
	);
}

type AddToCartPayload = {
	productID: number;
	quantity: number;
};

async function addToCart(payload: AddToCartPayload) {
	return ResultAsync.fromPromise(
		client.post(ENDPOINTS.CART, { json: payload }).json(), // Assuming the response is not used

		(error): CartError =>
			match(error as HTTPError)
				.with({ response: { status: 404 } }, () => ({
					type: "not_authenticated" as const,
				}))
				.otherwise(() => ({ type: "unknown_error" as const })),
	);
}

async function listCartItems() {
	return client.get(ENDPOINTS.CART).json<CartItemResponse[]>();
}

async function checkoutCart(payload: CheckoutPaymentPayload) {
	return client
		.post(`${ENDPOINTS.PAYMENTS}/checkout`, { json: payload })
		.json<CheckoutPaymentResponse>();
}
