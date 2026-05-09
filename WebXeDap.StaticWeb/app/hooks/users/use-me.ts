import { HTTPError } from "ky";
import { ResultAsync } from "neverthrow";
import { match, P } from "ts-pattern";
import { client } from "~/lib/httpClient";

interface MeResponse {
	id: number;
	email: string;
	firstName: string;
	lastName: string;
	image: string;
}

export type MeError = { type: "unknown_error"; message: string };

function getMe(): ResultAsync<MeResponse, MeError> {
	return ResultAsync.fromPromise(
		client.get("/auth/me").json<MeResponse>(),

		(error): MeError =>
			match(error)
				.with(P.instanceOf(HTTPError), (err) =>
					match(err.response.status).otherwise(() => ({
						type: "unknown_error" as const,
						message: err.message,
					})),
				)
				.otherwise(() => ({
					type: "unknown_error" as const,
					message: "Unexpected error occurred",
				})),
	);
}

export function useMe() {
	async function mutateAsync() {
		return await getMe();
	}

	return { mutateAsync };
}
