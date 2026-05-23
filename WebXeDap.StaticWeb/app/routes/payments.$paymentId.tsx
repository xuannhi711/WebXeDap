import { ArrowLeft, CreditCard, ExternalLink, RotateCcw } from "lucide-react";
import { Link } from "react-router";
import { Button } from "~/components/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle,
} from "~/components/ui/card";
import {
	formatPaymentProvider,
	formatPaymentStatus,
	usePayment,
} from "~/hooks/payments/use-payment";
import type { Route } from "./+types/payments.$paymentId";

function statusTone(status: number) {
	switch (status) {
		case 2:
			return "bg-emerald-500/15 text-emerald-700 ring-emerald-500/20";
		case 1:
			return "bg-amber-500/15 text-amber-700 ring-amber-500/20";
		case 3:
		case 4:
			return "bg-rose-500/15 text-rose-700 ring-rose-500/20";
		case 5:
			return "bg-sky-500/15 text-sky-700 ring-sky-500/20";
		default:
			return "bg-slate-500/15 text-slate-700 ring-slate-500/20";
	}
}

export default function PaymentPage({ params }: Route.ComponentProps) {
	const paymentID = Number.parseInt(params.paymentId, 10);
	const { payment, isLoading, error } = usePayment(paymentID);

	if (isLoading) {
		return <div className="p-6">Loading payment...</div>;
	}

	if (error || !payment) {
		return (
			<section className="px-6 py-10">
				<Card className="mx-auto max-w-2xl">
					<CardHeader>
						<CardTitle>Payment not found</CardTitle>
						<CardDescription>
							We could not load this payment record.
						</CardDescription>
					</CardHeader>
					<CardFooter className="justify-between gap-3">
						<Button asChild variant="outline">
							<Link to="/cart">
								<ArrowLeft /> Back to cart
							</Link>
						</Button>
					</CardFooter>
				</Card>
			</section>
		);
	}

	return (
		<section className="px-6 py-10">
			<div className="mx-auto grid max-w-5xl gap-6 lg:grid-cols-[1.3fr_0.7fr]">
				<Card className="overflow-hidden">
					<CardHeader className="border-b bg-gradient-to-br from-background to-muted/40">
						<div className="flex flex-wrap items-start justify-between gap-4">
							<div>
								<CardTitle className="text-2xl">Payment #{payment.id}</CardTitle>
								<CardDescription>
									Order #{payment.orderID} • Reference {payment.referenceCode}
								</CardDescription>
							</div>
							<span
								className={`inline-flex items-center rounded-full px-3 py-1 text-sm font-medium ring-1 ${statusTone(payment.status)}`}
							>
								{formatPaymentStatus(payment.status)}
							</span>
						</div>
					</CardHeader>
					<CardContent className="grid gap-6 p-6 md:grid-cols-2">
						<div className="space-y-4">
							<div className="rounded-xl border bg-muted/30 p-4">
								<div className="text-sm text-muted-foreground">Amount</div>
								<div className="mt-1 text-3xl font-bold">
									{payment.amount.toLocaleString()} {payment.currencyCode}
								</div>
							</div>
							<div className="grid gap-3 sm:grid-cols-2">
								<div className="rounded-xl border p-4">
									<div className="text-sm text-muted-foreground">Provider</div>
									<div className="mt-1 font-semibold">{formatPaymentProvider(payment.provider)}</div>
								</div>
								<div className="rounded-xl border p-4">
									<div className="text-sm text-muted-foreground">Completed</div>
									<div className="mt-1 font-semibold">
										{payment.completedAt ? new Date(payment.completedAt).toLocaleString() : "Not completed yet"}
									</div>
								</div>
							</div>
							<div className="rounded-xl border p-4">
								<div className="text-sm text-muted-foreground">Provider transaction ID</div>
								<div className="mt-1 break-all font-mono text-sm">
									{payment.providerTransactionID ?? "Pending"}
								</div>
							</div>
						</div>
						<div className="space-y-4">
							<div className="rounded-2xl border bg-gradient-to-b from-muted/40 to-background p-5">
								<div className="flex items-center gap-2 text-lg font-semibold">
									<CreditCard className="h-5 w-5" /> Payment status
								</div>
								<p className="mt-2 text-sm text-muted-foreground">
									{payment.status === 2
										? "Your payment was completed successfully."
										: payment.status === 1
											? "Your payment is being processed."
											: payment.status === 3
												? payment.failureReason ?? "The payment failed."
												: payment.status === 4
													? "The payment was cancelled."
													: "The payment is waiting for confirmation."}
								</p>
								{payment.providerPaymentUrl && payment.status !== 2 && (
									<Button className="mt-4 w-full" asChild>
										<a href={payment.providerPaymentUrl} target="_blank" rel="noreferrer">
											<ExternalLink /> Continue to payment provider
										</a>
									</Button>
								)}
								{payment.providerPaymentUrl && payment.status === 2 && (
									<p className="mt-4 text-sm text-muted-foreground">
										The provider link is still available in case you need to review it.
									</p>
								)}
							</div>
							<div className="rounded-2xl border p-5">
								<div className="text-sm font-semibold">What happens next</div>
								<ul className="mt-3 space-y-2 text-sm text-muted-foreground">
									<li>Keep this page open until the provider confirms the payment.</li>
									<li>You can return here any time using the payment ID.</li>
									<li>After success, the order is ready for fulfillment.</li>
								</ul>
							</div>
						</div>
					</CardContent>
					<CardFooter className="flex flex-wrap justify-between gap-3">
						<Button asChild variant="outline">
							<Link to="/cart">
								<ArrowLeft /> Back to cart
							</Link>
						</Button>
						<Button type="button" onClick={() => window.location.reload()}>
							<RotateCcw /> Refresh status
						</Button>
					</CardFooter>
				</Card>
			</div>
		</section>
	);
}