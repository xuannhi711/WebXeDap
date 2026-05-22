import { Link, useNavigate } from "react-router";
import {
	LoginForm,
	type LoginFormOnSubmitValidParams,
} from "~/components/forms/form-login";
import { Button, buttonVariants } from "~/components/ui/button";
import { FieldSeparator } from "~/components/ui/field";
import { useLogin, type LoginOption } from "~/hooks/users/use-login";
import { ROUTES } from "~/routes";

export default function LoginPage() {
	const login = useLogin();
	const navigate = useNavigate();

	async function onSubmitValidHandler(params: LoginFormOnSubmitValidParams) {
		const loginResult = await login.mutateAsync({
			type: "credentials",
			email: params.value.email,
			password: params.value.password,
		});

		if (loginResult.isErr()) {
			return {
				form: `Login failed: ${JSON.stringify(loginResult.error.message)}`,
				// fields: {
				// 	email: "Invalid email address",
				// },
			};
		}
		navigate(ROUTES.HOME);
	}

	return (
		<div className="grid lg:grid-cols-2 py-20 lg:px-50 px-20 gap-15">
			<div className="flex flex-col justify-between min-h-[67vh]">
				<div className="text-center">
					<h1 className="font-bold text-2xl">Login to your account</h1>
					<span className="mt-2 text-muted-foreground">
						Enter your email below to login to your account
					</span>
				</div>
				<LoginForm onSubmitValid={onSubmitValidHandler} />
				<FieldSeparator>Or continue with</FieldSeparator>
				<Oauth2LoginOptions />
				<p className="text-muted-foreground text-center">
					Don't have an account?{" "}
					<Link to={ROUTES.REGISTER} className="underline">
						Register
					</Link>
				</p>
			</div>
			<img
				loading="lazy"
				className="lg:block hidden size-full"
				src="https://picsum.photos/500/500"
				alt="banner"
			/>
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
