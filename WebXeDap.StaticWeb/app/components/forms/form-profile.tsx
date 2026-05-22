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
import { useEffect } from "react";

const PROFILE_FORM_SCHEMA = z.object({
	email: z.email({ message: "Invalid email address" }),
	fullName: z.string().nonempty({ message: "Full name is required" }),
	avatar: z.url({ message: "Invalid URL" }).optional(),
});

type ProfileFormValues = z.infer<typeof PROFILE_FORM_SCHEMA>;

export type ProfileFormOnSubmitValidParams = {
	value: ProfileFormValues;
	formApi: AnyFormApi;
};

type InvalidProfileFormError = {
	fields?: Partial<Record<keyof ProfileFormValues, string>>;
	form?: string;
};

interface ProfileFormProps extends React.ComponentProps<"form"> {
	className?: string;
	onSubmitValid?: (
		params: ProfileFormOnSubmitValidParams,
	) => Promise<undefined | InvalidProfileFormError>;
	defaultValues: ProfileFormValues;
}

export function ProfileForm({
	className,
	onSubmitValid,
	defaultValues,
	...props
}: ProfileFormProps) {
	const form = useForm({
		defaultValues: defaultValues,
		validators: {
			onSubmit: PROFILE_FORM_SCHEMA,
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
							{isInvalid && <FieldError errors={field.state.meta.errors} />}
						</Field>
					);
				}}
			</form.Field>

			<form.Field name="fullName">
				{(field) => {
					const isInvalid =
						field.state.meta.isTouched && !field.state.meta.isValid;
					return (
						<Field data-invalid={isInvalid}>
							<FieldLabel htmlFor={field.name}>Full Name</FieldLabel>
							<Input
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

			<form.Field name="avatar">
				{(field) => {
					const isInvalid =
						field.state.meta.isTouched && !field.state.meta.isValid;
					return (
						<Field data-invalid={isInvalid}>
							<FieldLabel htmlFor={field.name}>Avatar</FieldLabel>
							<Input
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
