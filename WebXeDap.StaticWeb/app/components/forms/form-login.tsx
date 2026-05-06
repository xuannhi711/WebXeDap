"use client";

import { useForm } from "@tanstack/react-form";
import { LoaderPinwheel } from "lucide-react";
import { Link } from "react-router";
import { z } from "zod";
import { Field, FieldError, FieldLabel } from "~/components/ui/field";
import { Input } from "~/components/ui/input";
import { PASSWORD_MAX_LENGTH, PASSWORD_MIN_LENGTH } from "~/config/authn";
import { cn } from "~/lib/utils";
import { Button } from "../ui/button";
import { InputPassword } from "../ui/input-password";
import { ROUTES } from "~/routes";

interface LoginFormProps extends React.ComponentProps<"form"> {
	className?: string;
	onSubmitValid?: (email: string, password: string) => Promise<void>;
}

const LOGIN_FORM_SCHEMA = z.object({
	email: z.email({ message: "Invalid email address" }),
	password: z
		.string()
		.min(PASSWORD_MIN_LENGTH, {
			message: `Password must be at least ${PASSWORD_MIN_LENGTH} characters`,
		})
		.max(PASSWORD_MAX_LENGTH, {
			message: `Password must be at most ${PASSWORD_MAX_LENGTH} characters`,
		}),
});

export function LoginForm({
	className,
	onSubmitValid,
	...props
}: LoginFormProps) {
	const form = useForm({
		defaultValues: {
			email: "",
			password: "",
		},
		validators: {
			onSubmit: LOGIN_FORM_SCHEMA,
		},
		onSubmit: async ({ value }) => {
			await onSubmitValid?.(value.email, value.password);
		},
	});

	return (
		<form
			className={cn(
				"group/field-group @container/field-group flex w-full flex-col gap-5 data-[slot=checkbox-group]:gap-3 *:data-[slot=field-group]:gap-4 overflow-hidden",
				className,
			)}
			onSubmit={(e) => {
				e.preventDefault();
				e.stopPropagation();
				form.handleSubmit();
			}}
			{...props}
		>
			<form.Field name="email">
				{(field) => {
					const isInvalid =
						field.state.meta.isTouched && !field.state.meta.isValid;
					return (
						<Field data-invalid={isInvalid}>
							<FieldLabel htmlFor={field.name}>Email</FieldLabel>
							<Input
								id={field.name}
								type="email"
								placeholder="johndoe@example.com"
								value={field.state.value}
								onBlur={field.handleBlur}
								onChange={(e) => field.handleChange(e.target.value)}
								aria-invalid={isInvalid}
								required
							/>
							{isInvalid && <FieldError errors={field.state.meta.errors} />}
						</Field>
					);
				}}
			</form.Field>

			<form.Field name="password">
				{(field) => {
					const isInvalid =
						field.state.meta.isTouched && !field.state.meta.isValid;
					return (
						<Field data-invalid={isInvalid}>
							<div className="flex items-center">
								<FieldLabel htmlFor={field.name}>Password</FieldLabel>
								<Link
									to={ROUTES.FORGOT_PASSWORD}
									className="ml-auto underline-offset-4 hover:underline"
								>
									Forgot password?
								</Link>
							</div>
							<InputPassword
								id={field.name}
								value={field.state.value}
								onBlur={field.handleBlur}
								onChange={(e) => field.handleChange(e.target.value)}
								aria-invalid={isInvalid}
								required
							/>
							{isInvalid && <FieldError errors={field.state.meta.errors} />}
						</Field>
					);
				}}
			</form.Field>

			<form.Subscribe selector={(state) => [state.isSubmitting]}>
				{([isSubmitting]) => (
					<Button type="submit" size="lg" disabled={isSubmitting}>
						{isSubmitting ? (
							<>
								<LoaderPinwheel className="animate-spin" />
								Logging in
							</>
						) : (
							"Login"
						)}
					</Button>
				)}
			</form.Subscribe>
		</form>
	);
}
