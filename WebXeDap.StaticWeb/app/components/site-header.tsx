import { PanelLeftIcon } from "lucide-react";
import { Link } from "react-router";
import { Button, buttonVariants } from "~/components/ui/button";
import { useSidebar } from "~/components/ui/sidebar";
import { ROUTES } from "~/routes";
import { useAuthnStore } from "~/store/store-authn";
import { NavUser } from "./sidebar-navs/nav-user";
import { SiteBrand } from "./site-brand";

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
	const user = useAuthnStore((state) => state.user);
	const isAuthenticated = !!user;

	if (!isAuthenticated) {
		return (
			<div className="flex flex-row items-center gap-2">
				<Link
					to={ROUTES.LOGIN}
					className={buttonVariants({
						variant: "outline",
						size: "lg",
					})}
				>
					Login
				</Link>
				<Link
					to={ROUTES.REGISTER}
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
