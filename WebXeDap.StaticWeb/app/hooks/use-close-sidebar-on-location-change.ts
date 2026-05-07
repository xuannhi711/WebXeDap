import { useEffect, useRef } from "react";
import { useLocation } from "react-router";
import { match, P } from "ts-pattern";
import { useSidebar } from "~/components/ui/sidebar";

export function useCloseSidebarOnLocationChange() {
	const location = useLocation();
	const { isMobile, openMobile, open, setOpen, setOpenMobile } = useSidebar();
	const previousPathname = useRef(location.pathname);

	useEffect(() => {
		if (previousPathname.current === location.pathname) {
			return;
		}

		previousPathname.current = location.pathname;

		match([isMobile, openMobile, open])
			.with([true, true, P._], () => setOpenMobile(false))
			.with([false, P._, true], () => setOpen(false))
			.otherwise(() => {});
	}, [isMobile, openMobile, open, setOpen, setOpenMobile, location.pathname]);
}
