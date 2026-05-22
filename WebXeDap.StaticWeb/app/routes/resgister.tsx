import { useState } from "react";
import { Link, useNavigate } from "react-router";
import { ResultAsync } from "neverthrow";
import { useRegister } from "~/hooks/users/use-register";

import {
	RegisterForm,
	type RegisterFormOnSubmitValidParams,
} from "~/components/forms/form-register";
import { Button, buttonVariants } from "~/components/ui/button";
import { FieldSeparator } from "~/components/ui/field";
import { ROUTES } from "~/routes";
import { useLogin } from "~/hooks/users/use-login";

interface UserService {
	register: (params: {
		email: string;
		password: string;
		confirmPassword: string;
	}) => Promise<{
		id: number;
		accessToken: string;
	}>;
}

const OATH2_OPTIONS = [
	{
		name: "Google",
		url: "/",
		icon: "https://www.svgrepo.com/show/303108/google-icon-logo.svg",
	},
	{
		name: "Meta",
		url: "/",
		icon: "https://www.svgrepo.com/show/431792/meta.svg",
	},
];

export default function RegisterPage() {
	const { mutateAsync: register } = useRegister();
	const navigate = useNavigate();

	async function onSubmitValidHandler(params: RegisterFormOnSubmitValidParams) {
		const registerResult = await register({
			email: params.value.email,
			password: params.value.password,
		});
		if (registerResult.isErr()) {
			const { errors } = registerResult.error.message as { errors: {} };
			return {
				form: `Registration failed: ${JSON.stringify(errors)}`,
			};
		}

		navigate(ROUTES.LOGIN);
		// setFormError(null);
		// await new Promise((resolve) => setTimeout(resolve, 1000));

		// const registerResult = await ResultAsync.fromPromise(
		// 	register(params.value),
		// 	(error) => "An unexpected error occurred",
		// );

		// if (registerResult.isOk()) {
		// 	const data = registerResult.value;
		// 	console.log("Registered user:", data);
		// } else {
		// 	const errorMessage = registerResult.error;
		// 	params.formApi.setErrorMap({ onSubmit: String(errorMessage) });
		// }
	}

	return (
		<div className="grid lg:grid-cols-2 py-20 lg:px-50 px-20 gap-15">
			<img
				loading="lazy"
				className="lg:block hidden size-full"
				src="https://picsum.photos/500/500"
				alt="banner"
			/>
			<div className="flex flex-col justify-between min-h-[67vh]">
				<div className="text-center">
					<h1 className="font-bold text-2xl">Create an account</h1>
					<span className="mt-2 text-muted-foreground">
						Enter your email below to create your account
					</span>
				</div>
				<RegisterForm onSubmitValid={onSubmitValidHandler} />
				<FieldSeparator>Or continue with</FieldSeparator>
				<Oauth2LoginOptions />
				<p className="text-muted-foreground text-center">
					Already have an account?{" "}
					<Link to={ROUTES.LOGIN} className="underline">
						Login
					</Link>
				</p>
			</div>
		</div>
	);
}

function Oauth2LoginOptions() {
	const login = useLogin();

	return (
		<div className="grid grid-cols-2 gap-4">
			<Button
				variant="outline"
				onClick={() => login.mutateAsync({ type: "google" })}
			>
				<img
					loading="lazy"
					src="https://www.svgrepo.com/show/303108/google-icon-logo.svg"
					alt="Google"
					className="h-5 w-5"
				/>
				Google
			</Button>
			<Button
				variant="outline"
				onClick={() => login.mutateAsync({ type: "google" })}
			>
				<img
					loading="lazy"
					src="https://www.svgrepo.com/show/431792/meta.svg"
					alt="Meta"
					className="h-5 w-5"
				/>
				Meta
			</Button>
		</div>
	);
}
