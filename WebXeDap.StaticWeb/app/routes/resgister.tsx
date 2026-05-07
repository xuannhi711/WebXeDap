import { useState } from "react";
import { Link } from "react-router";
import { RegisterForm } from "~/components/forms/form-register";
import { buttonVariants } from "~/components/ui/button";
import { FieldSeparator } from "~/components/ui/field";
import { ROUTES } from "~/routes";

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
	const [formError, setFormError] = useState<string | null>(null);
	const userService: UserService = {} as any;

	async function onSubmitValidHandler(
		email: string,
		password: string,
		confirmPassword: string,
	) {
		setFormError(null);
		await new Promise((resolve) => setTimeout(resolve, 1000));

		const registerData = await userService.register({
			email: email,
			password: password,
			confirmPassword: confirmPassword,
		});
	}

	function onSubmitErrorHandler(error: Error) {
		setFormError(error.message);
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
				<RegisterForm
					onSubmitValid={onSubmitValidHandler}
					onSubmitError={onSubmitErrorHandler}
				/>
				{formError && (
					<div className="text-destructive bg-destructive/10 rounded-md p-2 text-sm">
						{formError}
					</div>
				)}
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
					<img
						loading="lazy"
						src={option.icon}
						alt={option.name}
						className="h-5 w-5"
					/>
					{option.name}
				</Link>
			))}
		</div>
	);
}
