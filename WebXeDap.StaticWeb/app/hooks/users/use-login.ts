import { HTTPError } from "ky";
import { err, ok, ResultAsync } from "neverthrow";
import { match, P } from "ts-pattern";
import { client } from "~/lib/httpClient";
import { useStore } from "~/store/store";
import { useMe } from "./use-me";
import { ENDPOINTS } from "~/config/app";

interface LoginPayload {
	email: string;
	password: string;
}

interface LoginResponse {
	// accessToken: string;
	// refreshToken: string;
}

export type LoginError =
	| { type: "invalid_credentials"; message: string }
	| { type: "unknown_error"; message: string };

async function login(payload: LoginPayload) {
	return ResultAsync.fromPromise(
		client.post(ENDPOINTS.LOGIN, {
			json: payload,
		}),

		(error) =>
			match(error)
				.with(P.instanceOf(HTTPError), (err) =>
					match(err.response.status)
						.with(400, () => ({
							type: "invalid_credentials" as const,
							message: err.data,
						}))
						.otherwise(() => ({
							type: "unknown_error" as const,
							message: err.data,
						})),
				)
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error occurred",
				})),
	);
}

export function useLogin() {
	const { setToken, setUser, clearAuthnState, setAuthenticated } = useStore();
	const me = useMe();

	async function mutateAsync(payload: LoginPayload) {
		const loginResult = await login(payload);
		if (loginResult.isErr()) {
			return err(loginResult.error);
		}
		console.log("pass");

		// const { accessToken, refreshToken } = loginResult.value;
		// setToken(accessToken, refreshToken);

		const meResult = await me.mutateAsync();
		if (meResult.isErr()) {
			clearAuthnState();
			return err(meResult.error);
		}
		const userData = meResult.value;
		setUser({ ...userData, avatar: userData.image });
		setAuthenticated(true);
		return ok();
	}

	return {
		mutateAsync,
	};
}
