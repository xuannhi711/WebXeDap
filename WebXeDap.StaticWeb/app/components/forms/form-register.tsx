"use client";

import { type AnyFormApi, useForm } from "@tanstack/react-form";
import { LoaderPinwheel } from "lucide-react";
import { match } from "ts-pattern";
import { z } from "zod";
import { Field, FieldError, FieldLabel } from "~/components/ui/field";
import { Input } from "~/components/ui/input";
import { PASSWORD_MAX_LENGTH, PASSWORD_MIN_LENGTH } from "~/config/authn";
import { cn } from "~/lib/utils";
import { Button } from "../ui/button";
import { InputPassword } from "../ui/input-password";

const REGISTER_FORM_SCHEMA = z
	.object({
		email: z.email({ message: "Invalid email address" }),
		password: z
			.string()
			.min(PASSWORD_MIN_LENGTH, {
				message: `Password must be at least ${PASSWORD_MIN_LENGTH} characters`,
			})
			.max(PASSWORD_MAX_LENGTH, {
				message: `Password must be at most ${PASSWORD_MAX_LENGTH} characters`,
			}),
		confirmPassword: z.string(),
	})
	.superRefine((data, ctx) => {
		if (data.password === data.confirmPassword) {
			return;
		}
		ctx.addIssue({
			code: "custom",
			path: ["confirmPassword"],
			message: "Passwords do not match",
		});
	});

type RegisterFormValues = z.infer<typeof REGISTER_FORM_SCHEMA>;

export type RegisterFormOnSubmitValidParams = {
	value: RegisterFormValues;
	formApi: AnyFormApi;
};

interface RegisterFormProps extends React.ComponentProps<"form"> {
	className?: string;
	onSubmitValid?: (params: RegisterFormOnSubmitValidParams) => Promise<void>;
}

const REGISTER_FORM_DEFAULT_VALUES: RegisterFormValues = {
	email: "",
	password: "",
	confirmPassword: "",
};

export function RegisterForm({
	className,
	onSubmitValid,
	...props
}: RegisterFormProps) {
	const form = useForm({
		defaultValues: REGISTER_FORM_DEFAULT_VALUES,
		validators: {
			onSubmit: REGISTER_FORM_SCHEMA,
		},
		onSubmit: onSubmitValid,
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
							<FieldLabel htmlFor={field.name}>Password</FieldLabel>
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

			<form.Field name="confirmPassword">
				{(field) => {
					const isInvalid =
						field.state.meta.isTouched && !field.state.meta.isValid;
					return (
						<Field data-invalid={isInvalid}>
							<FieldLabel htmlFor={field.name}>Confirm Password</FieldLabel>
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
						{match(isSubmitting)
							.with(true, () => (
								<>
									<LoaderPinwheel className="animate-spin" />
									Registering
								</>
							))
							.otherwise(() => (
								<>Register</>
							))}
					</Button>
				)}
			</form.Subscribe>
		</form>
	);
}
