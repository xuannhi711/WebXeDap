import { useMutation, useQuery } from "@tanstack/react-query";
import { HTTPError } from "ky";
import { err, ok, ResultAsync } from "neverthrow";
import { match } from "ts-pattern";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";

interface ChangePasswordPayload {
	oldPassword: string;
	newPassword: string;
}
export function useChangePassword() {
	const mutation = useMutation({
		mutationFn: async (payload: ChangePasswordPayload) => {
			await client.post(ENDPOINTS.AUTH_INFO, { json: payload });
		},
	});

	return mutation;
}
