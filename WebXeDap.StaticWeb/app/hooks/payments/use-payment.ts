import { useQuery } from "@tanstack/react-query";
import type { HTTPError } from "ky";
import { ResultAsync } from "neverthrow";
import { match } from "ts-pattern";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";

export type PaymentStatus = 0 | 1 | 2 | 3 | 4 | 5;

export type PaymentProvider = 0 | 1 | 2 | 3 | 4;

export interface PaymentResponse {
	id: number;
	orderID: number;
	provider: PaymentProvider;
	status: PaymentStatus;
	amount: number;
	currencyCode: string;
	referenceCode: string;
	providerTransactionID?: string | null;
	providerPaymentUrl?: string | null;
	failureReason?: string | null;
	completedAt?: string | null;
}

export function formatPaymentStatus(status: PaymentStatus) {
	return {
		0: "Pending",
		1: "Processing",
		2: "Succeeded",
		3: "Failed",
		4: "Cancelled",
		5: "Refunded",
	}[status];
}

export function formatPaymentProvider(provider: PaymentProvider) {
	return {
		0: "Unknown",
		1: "VNPay",
		2: "Stripe",
		3: "PayPal",
		4: "Manual",
	}[provider];
}

type PaymentError =
	| { type: "payment_not_found"; message: string }
	| { type: "unknown_error"; message: string };

function fetchPayment(id: number) {
	return ResultAsync.fromPromise(
		client.get(`${ENDPOINTS.PAYMENTS}/${id}`).json<PaymentResponse>(),
		(error): PaymentError =>
			match(error as HTTPError)
				.with({ response: { status: 404 } }, () => ({
					type: "payment_not_found" as const,
					message: "Payment not found",
				}))
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error occurred",
				})),
	);
}

export function usePayment(id: number) {
	const { data, isLoading } = useQuery({
		queryKey: ["payment", id],
		queryFn: async () => await fetchPayment(id),
		enabled: Number.isFinite(id) && id > 0,
	});

	const payment = data?.unwrapOr(undefined);
	const error = data?.isErr() ? data.error : undefined;

	return { payment, isLoading, error };
}