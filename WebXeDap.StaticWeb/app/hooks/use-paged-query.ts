import { useMemo } from "react";
import { useLocation, useNavigate } from "react-router";

export function usePagedQuery(defaultPage = 1, defaultSize = 20) {
	const location = useLocation();
	const navigate = useNavigate();

	const { page, size } = useMemo(() => {
		const params = new URLSearchParams(location.search);
		const p = parseInt(params.get("page") ?? "", 10);
		const s = parseInt(params.get("size") ?? "", 10);
		return {
			page: Number.isFinite(p) && p > 0 ? p : defaultPage,
			size: Number.isFinite(s) && s > 0 ? s : defaultSize,
		};
	}, [location.search, defaultPage, defaultSize]);

	function setPaged(newPage: number, newSize?: number) {
		const params = new URLSearchParams(location.search);
		params.set("page", String(newPage));
		if (newSize !== undefined) params.set("size", String(newSize));
		navigate({ pathname: location.pathname, search: params.toString() });
	}

	return { page, size, setPaged };
}
