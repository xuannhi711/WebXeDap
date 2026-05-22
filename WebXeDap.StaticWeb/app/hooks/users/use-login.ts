import { HTTPError } from "ky";
import { err, ok, ResultAsync } from "neverthrow";
import { match } from "ts-pattern";
import { API_HOST, ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";
import { useMe } from "./use-me";
import type { MergeExclusive, Except } from "type-fest";

type CredentialLogin = {
	type: "credentials";
	email: string;
	password: string;
};

type GoogleLogin = {
	type: "google";
};

type LoginPayload = MergeExclusive<CredentialLogin, GoogleLogin>;

export type LoginOption = LoginPayload["type"];

// interface LoginResponse {
// 	// accessToken: string;
// 	// refreshToken: string;
// }

export function useLogin() {
	const me = useMe();

	async function mutateAsync(payload: LoginPayload) {
		const loginResult = await match(payload)
			.with({ type: "credentials" }, async (cred) => await login(cred))
			.with({ type: "google" }, async () => await loginWithGoogle())
			.exhaustive();

		if (loginResult.isErr()) {
			return err(loginResult.error);
		}

		const meResult = await me.mutateAsync();
		if (meResult.isErr()) {
			return err(meResult.error);
		}
		return ok();
	}

	return {
		mutateAsync,
	};
}

export type LoginError =
	| { type: "invalid_credentials"; message: string }
	| { type: "unknown_error"; message: string };

async function login(payload: Except<CredentialLogin, "type">) {
	return ResultAsync.fromPromise(
		client.post(ENDPOINTS.LOGIN, {
			json: payload,
		}),

		(error) =>
			match(error as HTTPError)
				.with({ response: { status: 400 } }, (err) => ({
					type: "invalid_credentials" as const,
					message: err.data,
				}))
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error occurred",
				})),
	);
}

async function loginWithGoogle() {
	const popup = window.open(
		`${API_HOST}${ENDPOINTS.LOGIN_W_GGL}?origin=${encodeURIComponent(window.location.origin)}`,
		"_self",
		"width=500,height=600",
	);

	if (!popup) {
		return err({
			type: "unknown_error",
			message: "Failed to open login window",
		} as const);
	}

	return ok();
}
