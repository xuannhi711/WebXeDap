import { HTTPError } from "ky";
import { err, ok, ResultAsync } from "neverthrow";
import { match } from "ts-pattern";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";
import { useStore } from "~/store/store";

export function useMe() {
	const { setUser, clearAuthnState, setAuthenticated } = useStore();

	async function mutateAsync() {
		const res = await fetchMe();

		if (res.isErr()) {
			clearAuthnState();
			return err(res.error);
		}
		const userData = res.value;
		setUser(userData);
		setAuthenticated(true);
		return ok(userData);
	}

	return { mutateAsync };
}

export type MeError =
	| { type: "user_not_found"; message: string }
	| { type: "unknown_error"; message: string };

interface MeResponse {
	id: number;
	email: string;
	fullName: string;
	avatar: string;
}

function fetchMe() {
	return ResultAsync.fromPromise(
		client.get(ENDPOINTS.ME).json<MeResponse>(),

		(error): MeError =>
			match(error as HTTPError)
				.with({ response: { status: 404 } }, () => ({
					type: "user_not_found" as const,
					message: "User not found",
				}))
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error occurred",
				})),
	);
}
