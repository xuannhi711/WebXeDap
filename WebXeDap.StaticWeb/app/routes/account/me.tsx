import { Eye, Lock, Pen, User, ReceiptText } from "lucide-react";
import { useMemo, useState } from "react";
import {
	ProfileForm,
	type ProfileFormOnSubmitValidParams,
} from "~/components/forms/form-profile";
import { Button } from "~/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "~/components/ui/tabs";
import {
	Card,
	CardAction,
	CardContent,
	CardFooter,
	CardHeader,
	CardTitle,
} from "~/components/ui/card";
import { useLogout } from "~/hooks/users/use-logout";
import { useMe } from "~/hooks/users/use-me";
import { useStore } from "~/store/store";
import {
	PasswordForm,
	type PasswordFormOnSubmitValidParams,
} from "~/components/forms/form-password";
import { useChangePassword } from "~/hooks/users/use-change-password";
import { useUpdateMe } from "~/hooks/users/use-update-me";
import { HTTPError } from "ky";
import { ResultAsync } from "neverthrow";

export function meta() {
	return [{ title: "Wheelie | Account" }];
}

export default function Me() {
	return (
		<Tabs defaultValue="account-details" className="mx-30 my-10">
			<TabsList className="bg-transparent border-b-2 p-5">
				<TabsTrigger className="text-lg p-4" value="account-details">
					<User />
					Account Details
				</TabsTrigger>
				<TabsTrigger className="text-lg p-4" value="security">
					<Lock />
					Security
				</TabsTrigger>
				<TabsTrigger className="text-lg p-4" value="orders">
					<ReceiptText />
					Orders
				</TabsTrigger>
			</TabsList>
			<Card>
				<TabsContent value="account-details">
					<AccountDetailsTab />
				</TabsContent>
				<TabsContent value="security">
					<SecurityTab />
				</TabsContent>
				<TabsContent value="orders">
					<OrdersTab />
				</TabsContent>
			</Card>
		</Tabs>
	);
}

function SecurityTab() {
	const { mutateAsync } = useChangePassword();

	async function onSubmitValidPasswordHandler(
		params: PasswordFormOnSubmitValidParams,
	) {
		try {
			await mutateAsync({
				oldPassword: params.value.oldPassword,
				newPassword: params.value.newPassword,
			});
			params.formApi.reset();
		} catch (error) {
			const err = error as HTTPError;
			const { errors } = err.data as { errors: any };
			return {
				form: `Change password failed: ${JSON.stringify(errors)}`,
			};
		}
	}

	return (
		<>
			<CardHeader>
				<CardTitle className="text-2xl font-bold">SECURITY</CardTitle>
			</CardHeader>
			<CardContent>
				<PasswordForm onSubmitValid={onSubmitValidPasswordHandler} />
			</CardContent>
		</>
	);
}

function AccountDetailsTab() {
	const { email, fullName, avatar } = useStore((state) => state);
	const defaultValues = useMemo(
		() => ({
			email: email ?? "",
			fullName: fullName ?? "",
			avatar: avatar ?? "",
		}),
		[email, fullName, avatar],
	);
	const [isEditing, setIsEditing] = useState(false);
	const logout = useLogout();
	const { mutateAsync } = useUpdateMe();

	async function onSubmitValidProfileHandler(
		params: ProfileFormOnSubmitValidParams,
	) {
		try {
			await mutateAsync({
				fullName: params.value.fullName,
				avatar: params.value.avatar,
			});
			setIsEditing(false);
		} catch (error) {
			const err = error as HTTPError;
			const { errors } = err.data as { errors: any };
			return {
				form: `Update failed: ${JSON.stringify(errors)}`,
			};
		}
	}

	return (
		<>
			<CardHeader>
				<CardTitle className="text-2xl font-bold">ACCOUNT DETAILS</CardTitle>
				<CardAction>
					<Button
						variant={isEditing ? "default" : "outline"}
						onClick={() => setIsEditing((prev) => !prev)}
					>
						{!isEditing ? (
							<>
								View
								<Eye />
							</>
						) : (
							<>
								Edit
								<Pen />
							</>
						)}
					</Button>
				</CardAction>
			</CardHeader>
			<CardContent>
				<ProfileForm
					className="inert:scale-95 transition"
					inert={!isEditing}
					defaultValues={defaultValues}
					onSubmitValid={onSubmitValidProfileHandler}
				/>
			</CardContent>
			<CardFooter className="flex-col gap-2">
				<Button
					variant="destructive"
					className="w-full"
					onClick={logout.mutateAsync}
				>
					Logout
				</Button>
			</CardFooter>
		</>
	);
}

function OrdersTab() {
	return (
		<>
			<CardHeader>
				<CardTitle className="text-2xl font-bold">ORDERS</CardTitle>
			</CardHeader>
			<CardContent>saysum</CardContent>
		</>
	);
}
