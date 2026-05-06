import { useState } from "react";
import { Link } from "react-router";
import { LoginForm } from "~/components/forms/form-login";
import { buttonVariants } from "~/components/ui/button";
import { FieldSeparator } from "~/components/ui/field";
import { ROUTES } from "~/routes";
import { type AuthenticatedUser, setAuthnState } from "~/store/authn-slice";
import { useAppDispatch } from "~/store/hooks";

interface AuthnService {
	login: (params: { email: string; password: string }) => Promise<{
		id: number;
		accessToken: string;
	}>;

	me: (params: { accessToken: string }) => Promise<AuthenticatedUser>;
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

export default function LoginPage() {
	const dispatch = useAppDispatch();
	const [formError, setFormError] = useState<string | null>(null);
	const authnService: AuthnService = {} as any;

	async function onSubmitValidHandler(email: string, password: string) {
        setFormError(null);
		await new Promise((resolve) => setTimeout(resolve, 1000));

		const loginData = await authnService.login({
			email: email,
			password: password,
		});

		const meData = await authnService.me({
			accessToken: loginData.accessToken,
		});

		dispatch(
			setAuthnState({
				user: meData,
				...loginData,
			}),
		);
	}

	function onSubmitErrorHandler(error: Error) {
		setFormError(error.message);
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
				<LoginForm
					onSubmitValid={onSubmitValidHandler}
					onSubmitError={onSubmitErrorHandler}
				/>
				{formError && (
					<div className="text-destructive bg-destructive/10 rounded-md p-2 text-sm">{formError}</div>
				)}
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
				className="lg:block hidden size-full"
				src="https://picsum.photos/500/500"
				alt="banner"
			/>
		</div>
	);
}

function Oauth2LoginOptions() {
	return (
		<div className="grid grid-cols-2 gap-4">
			{OATH2_OPTIONS.map((option) => (
				<Link
					key={option.name}
					to={option.url}
					className={buttonVariants({
						variant: "outline",
					})}
				>
					<img src={option.icon} alt={option.name} className="h-5 w-5" />
					{option.name}
				</Link>
			))}
		</div>
	);
}
