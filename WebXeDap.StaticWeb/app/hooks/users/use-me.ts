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

	async function updateAsync(payload: UpdateMePayload) {
		const res = await updateMe(payload);
		if (res.isErr()) {
			return err(res.error);
		}
		const updatedUserData = res.value;
		setUser(updatedUserData);
		return ok(updatedUserData);
	}

	return { mutateAsync, updateAsync };
}

export type MeError =
	| { type: "user_not_found"; message: string }
	| { type: "validation_error"; message: unknown }
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

interface UpdateMePayload {
	fullName?: string;
	avatar?: string;
}

async function updateMe(payload: UpdateMePayload) {
	return ResultAsync.fromPromise(
		client.put(ENDPOINTS.ME, { json: payload }).json<MeResponse>(),

		(error): MeError =>
			match(error as HTTPError)
				.with({ response: { status: 404 } }, () => ({
					type: "user_not_found" as const,
					message: "User not found",
				}))
				.with({ response: { status: 400 } }, (err) => ({
					type: "validation_error" as const,
					message: err.data,
				}))
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error occurred",
				})),
	);
}
