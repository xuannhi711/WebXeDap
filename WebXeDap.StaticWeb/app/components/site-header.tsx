import { PanelLeftIcon } from "lucide-react";
import { Button, buttonVariants } from "~/components/ui/button";
import { useSidebar } from "~/components/ui/sidebar";
import { NavUser } from "./sidebar-navs/nav-user";
import { SiteBrand } from "./site-brand";
import { useAppSelector } from "~/store/hooks";
import { Link } from "react-router";

export function SiteHeader() {
	const { toggleSidebar } = useSidebar();

	return (
		<header className="sticky top-0 z-50 flex w-full items-center border-b bg-background">
			<div className="flex h-(--header-height) w-full items-center gap-4 px-4">
				<Button
					className="h-8 w-8"
					variant="ghost"
					size="icon"
					onClick={toggleSidebar}
				>
					<PanelLeftIcon />
				</Button>
				<SiteBrand />
				<div className="ml-auto">
					<UserMenu />
				</div>
			</div>
		</header>
	);
}

function UserMenu() {
	const user = useAppSelector((state) => state.auth.user);
	const isAuthenticated = !!user;

	if (!isAuthenticated) {
		return (
			<div className="flex flex-row items-center gap-2">
				<Link
					to="/login"
					className={buttonVariants({
						variant: "outline",
						size: "lg",
					})}
				>
					Login
				</Link>
				<Link
					to="/register"
					className={buttonVariants({
						variant: "default",
						size: "lg",
					})}
				>
					Register
				</Link>
			</div>
		);
	}
	return <NavUser />;
}
