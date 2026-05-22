import { HTTPError } from "ky";
import { err, ok, ResultAsync } from "neverthrow";
import { match, P } from "ts-pattern";
import { client } from "~/lib/httpClient";
import { ENDPOINTS } from "~/config/app";
import { useMe } from "./use-me";
import { useStore } from "~/store/store";
import { useNavigate } from "react-router";
import { ROUTES } from "~/routes";

interface RegisterPayload {
	email: string;
	password: string;
}

export type RegisterError = { type: "unknown_error"; message: string };

function register(payload: RegisterPayload) {
	return ResultAsync.fromPromise(
		client.post(ENDPOINTS.REGISTER, { json: payload }),
		(error) =>
			match(error)
				.with(P.instanceOf(HTTPError), (err) => ({
					type: "unknown_error" as const,
					message: err.data,
				}))
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error",
				})),
	);
}

export function useRegister() {
	const me = useMe();

	async function mutateAsync(payload: RegisterPayload) {
		const res = await register(payload);
		if (res.isErr()) return err(res.error);

		return ok();
	}

	return { mutateAsync };
}
