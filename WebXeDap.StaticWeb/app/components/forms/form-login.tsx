"use client";

import { useForm } from "@tanstack/react-form";
import { LoaderPinwheel } from "lucide-react";
import { Link } from "react-router";
import { match } from "ts-pattern";
import { z } from "zod";
import { Field, FieldLabel } from "~/components/ui/field";
import { Input } from "~/components/ui/input";
import { PASSWORD_MAX_LENGTH, PASSWORD_MIN_LENGTH } from "~/config/authn";
import { cn } from "~/lib/utils";
import { ROUTES } from "~/routes";
import { Button } from "../ui/button";
import { InputPassword } from "../ui/input-password";

const LOGIN_FORM_SCHEMA = z.object({
	email: z.string({ message: "Invalid email address" }),
	password: z
		.string()
		.min(PASSWORD_MIN_LENGTH, {
			message: `Password must be at least ${PASSWORD_MIN_LENGTH} characters`,
		})
		.max(PASSWORD_MAX_LENGTH, {
			message: `Password must be at most ${PASSWORD_MAX_LENGTH} characters`,
		}),
});

type LoginFormValues = z.infer<typeof LOGIN_FORM_SCHEMA>;

export type LoginFormOnSubmitValidParams = {
	value: LoginFormValues;
};

type InvalidLoginFormError = {
	fields?: Partial<Record<keyof LoginFormValues, string>>;
	form?: string;
};

interface LoginFormProps extends React.ComponentProps<"form"> {
	className?: string;
	onSubmitValid?: (
		params: LoginFormOnSubmitValidParams,
	) => Promise<undefined | InvalidLoginFormError>;
}

const LOGIN_FORM_DEFAULT_VALUES: LoginFormValues = {
	email: "",
	password: "",
};

export function LoginForm({
	className,
	onSubmitValid,
	...props
}: LoginFormProps) {
	const form = useForm({
		defaultValues: LOGIN_FORM_DEFAULT_VALUES,
		validators: {
			onSubmit: LOGIN_FORM_SCHEMA,
			onSubmitAsync: onSubmitValid,
		},
	});

	return (
		<form
			className={cn(
				"group/field-group @container/field-group flex w-full flex-col gap-5 data-[slot=checkbox-group]:gap-3 *:data-[slot=field-group]:gap-4",
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
							{isInvalid && <p>{field.state.meta.errors.join(", ")}</p>}
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
							{isInvalid && <p>{field.state.meta.errors.join(", ")}</p>}
						</Field>
					);
				}}
			</form.Field>

			<form.Subscribe selector={(state) => [state.isSubmitting]}>
				{([isSubmitting]) => (
					<Button type="submit" size="lg" disabled={isSubmitting}>
						{match(isSubmitting)
							.with(true, () => (
								<>
									<LoaderPinwheel className="animate-spin" />
									Logging in
								</>
							))
							.otherwise(() => (
								<>Login</>
							))}
					</Button>
				)}
			</form.Subscribe>

			<form.Subscribe selector={(state) => [state.errorMap.onSubmit]}>
				{([formError]) =>
					formError && (
						<div className="text-destructive bg-destructive/10 rounded-md p-2 text-sm">
							{formError.form as string}
						</div>
					)
				}
			</form.Subscribe>
		</form>
	);
}
