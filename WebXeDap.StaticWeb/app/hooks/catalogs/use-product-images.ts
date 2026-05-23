import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { client } from "~/lib/httpClient";
import { ENDPOINTS } from "~/config/app";

export type ProductImage = { id: number; url: string; order: number };

export function useProductImages(productId: number) {
	return useQuery({
		queryKey: ["productImages", productId],
		queryFn: async () =>
			await client
				.get(`${ENDPOINTS.PRODUCTS}/${productId}/images`)
				.json<ProductImage[]>(),
		enabled: !!productId,
	});
}

export function useUploadProductImage(productId: number) {
	const qc = useQueryClient();
	return useMutation({
		mutationFn: async (file: File) => {
			const fd = new FormData();
			fd.append("file", file);
			const res = await client
				.post(`${ENDPOINTS.PRODUCTS}/${productId}/images`, { body: fd })
				.json();
			return res;
		},
		onSuccess() {
			qc.invalidateQueries(["productImages", productId]);
		},
	});
}

export function useDeleteProductImage(productId: number) {
	const qc = useQueryClient();
	return useMutation({
		mutationFn: async (imageId: number) => {
			await client.delete(
				`${ENDPOINTS.PRODUCTS}/${productId}/images/${imageId}`,
			);
		},
		onSuccess() {
			qc.invalidateQueries(["productImages", productId]);
		},
	});
}
