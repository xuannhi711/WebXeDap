import ky from "ky";
import { API_HOST, REQUEST_TIMEOUT } from "~/config/app";
import { useStore } from "~/store/store";

export const client = ky.create({
	baseUrl: API_HOST,
	credentials: "include",
	timeout: REQUEST_TIMEOUT,
	hooks: {
		beforeRequest: [
			({ request }) => {
				const accessToken = useStore.getState().accessToken;
				if (!accessToken) {
					return;
				}
				request.headers.set("Authorization", `Bearer ${accessToken}`);
			},
		],
	},
});
