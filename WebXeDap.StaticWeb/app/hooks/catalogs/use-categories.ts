import { useQuery } from "@tanstack/react-query";
import { ENDPOINTS } from "~/config/app";
import { client } from "~/lib/httpClient";

export interface HierarchyCategory {
	id: number;
	name: string;
	children?: HierarchyCategory[];
}

async function fetchHierarchy() {
	return client
		.get(`${ENDPOINTS.CATEGORIES}/hierarchy`)
		.json<HierarchyCategory[]>();
}

export function useCategories() {
	const { data, isLoading } = useQuery({
		queryKey: ["categories", "hierarchy"],
		queryFn: fetchHierarchy,
	});

	return { categories: data ?? [], isLoading };
}
