import { HTTPError } from "ky";
import { err, ok, ResultAsync } from "neverthrow";
import { useNavigate } from "react-router";
import { match } from "ts-pattern";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";
import { useStore } from "~/store/store";

export type LogoutError =
	| { type: "not_authenticated" }
	| { type: "unknown_error" };

export function useLogout() {
	const { clearAuthnState, clearUser } = useStore();
	const navigate = useNavigate();

	async function mutateAsync() {
		const res = await logout();
		if (res.isErr()) {
			return err(res.error);
		}
		clearAuthnState();
		clearUser();
		return navigate("/");
	}

	return { mutateAsync };
}

async function logout() {
	return ResultAsync.fromPromise(
		client.post(ENDPOINTS.LOGOUT),

		(error) =>
			match(error as HTTPError)
				.with({ response: { status: 404 } }, () => ({
					type: "not_authenticated" as const,
				}))
				.otherwise(() => ({ type: "unknown_error" as const })),
	);
}
