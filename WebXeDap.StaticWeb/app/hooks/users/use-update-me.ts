import { useMutation } from "@tanstack/react-query";
import type { HTTPError } from "ky";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";
import { useStore } from "~/store/store";

interface MeResponse {
	id: number;
	email: string;
	fullName: string;
	avatar: string;
}
interface UpdateMePayload {
	fullName?: string;
	avatar?: string;
}

export function useUpdateMe() {
	const { setUser } = useStore();

	const mutation = useMutation<void, HTTPError, UpdateMePayload>({
		mutationFn: async (payload: UpdateMePayload) => {
			const res = await client
				.put(ENDPOINTS.ME, { json: payload })
				.json<MeResponse>();
			setUser(res);
		},
	});

	return mutation;
}
