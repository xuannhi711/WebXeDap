import { HTTPError } from "ky";
import { err, ok, ResultAsync } from "neverthrow";
import { match, P } from "ts-pattern";
import { client } from "~/lib/httpClient";
import { ENDPOINTS } from "~/config/app";
import { useMe } from "./use-me";
import { useStore } from "~/store/store";

interface RegisterPayload {
	email: string;
	password: string;
	fullName?: string;
}

export type RegisterError = { type: "unknown_error"; message: string };

function register(payload: RegisterPayload) {
	return ResultAsync.fromPromise(
		client.post(ENDPOINTS.REGISTER, { json: payload }),
		(error) =>
			match(error)
				.with(P.instanceOf(HTTPError), (err) => ({
					type: "unknown_error" as const,
					message: err.message,
				}))
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error",
				})),
	);
}

export function useRegister() {
	const me = useMe();
	const { setUser, setAuthenticated } = useStore();

	async function mutateAsync(payload: RegisterPayload) {
		const res = await register(payload);
		if (res.isErr()) return err(res.error);

		// Try to fetch current user; MapIdentityApi may sign-in on register
		const meResult = await me.mutateAsync();
		if (meResult.isErr()) return ok(undefined); // registration succeeded even if me fetch failed

		setUser(meResult.value);
		setAuthenticated(true);
		return ok(undefined);
	}

	return { mutateAsync };
}
