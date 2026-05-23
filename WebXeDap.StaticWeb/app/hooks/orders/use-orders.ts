import { useQuery } from "@tanstack/react-query";
import { client } from "~/lib/httpClient";
import { ENDPOINTS, API_HOST } from "~/config/app";

export function useOrders() {
	return useQuery({
		queryKey: ["orders"],
		queryFn: async () => await client.get(`${API_HOST}/api/orders`).json(),
	});
}
