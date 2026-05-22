"use client";

import { type AnyFormApi, useForm } from "@tanstack/react-form";
import { LoaderPinwheel } from "lucide-react";
import { match } from "ts-pattern";
import { z } from "zod";
import { Field, FieldError, FieldLabel } from "~/components/ui/field";
import { PASSWORD_MAX_LENGTH, PASSWORD_MIN_LENGTH } from "~/config/authn";
import { cn } from "~/lib/utils";
import { Button } from "../ui/button";
import { InputPassword } from "../ui/input-password";

const PASSWORD_FORM_SCHEMA = z
	.object({
		oldPassword: z.string(),
		newPassword: z
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
		if (data.newPassword === data.confirmPassword) {
			return;
		}
		ctx.addIssue({
			code: "custom",
			path: ["confirmPassword"],
			message: "Passwords do not match",
		});
	});

type PasswordFormValues = z.infer<typeof PASSWORD_FORM_SCHEMA>;

export type PasswordFormOnSubmitValidParams = {
	value: PasswordFormValues;
	formApi: AnyFormApi;
};

type InvalidPasswordFormError = {
	fields?: Partial<Record<keyof PasswordFormValues, string>>;
	form?: string;
};

interface PasswordFormProps extends React.ComponentProps<"form"> {
	className?: string;
	onSubmitValid?: (
		params: PasswordFormOnSubmitValidParams,
	) => Promise<undefined | InvalidPasswordFormError>;
}

const PASSWORD_FORM_DEFAULT_VALUES: PasswordFormValues = {
	oldPassword: "",
	newPassword: "",
	confirmPassword: "",
};

export function PasswordForm({
	className,
	onSubmitValid,
	...props
}: PasswordFormProps) {
	const form = useForm({
		defaultValues: PASSWORD_FORM_DEFAULT_VALUES,
		validators: {
			onSubmit: PASSWORD_FORM_SCHEMA,
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
			<form.Field name="oldPassword">
				{(field) => {
					const isInvalid =
						field.state.meta.isTouched && !field.state.meta.isValid;
					return (
						<Field data-invalid={isInvalid}>
							<FieldLabel htmlFor={field.name}>Old Password</FieldLabel>
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

			<form.Field name="newPassword">
				{(field) => {
					const isInvalid =
						field.state.meta.isTouched && !field.state.meta.isValid;
					return (
						<Field data-invalid={isInvalid}>
							<FieldLabel htmlFor={field.name}>New Password</FieldLabel>
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
									Updating
								</>
							))
							.otherwise(() => (
								<>Update</>
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
