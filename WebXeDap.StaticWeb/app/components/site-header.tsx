import { PanelLeftIcon } from "lucide-react";
import { Link } from "react-router";
import { Button, buttonVariants } from "~/components/ui/button";
import { useSidebar } from "~/components/ui/sidebar";
import { ROUTES } from "~/routes";
import { NavUser } from "./sidebar-navs/nav-user";
import { SiteBrand } from "./site-brand";
import { useStore } from "~/store/store";
import { NavCart } from "./sidebar-navs/nav-cart";

export function SiteHeader() {
	const { toggleSidebar } = useSidebar();

	return (
		<header className="sticky top-0 z-50 flex w-full items-center border-b bg-background">
			<div className="grid grid-cols-3 h-(--header-height) items-center w-full gap-4 px-4">
				<div className="flex items-center gap-6 text-sm font-medium">
					<Button
						className="h-8 w-8"
						variant="ghost"
						size="icon"
						onClick={toggleSidebar}
					>
						<PanelLeftIcon />
					</Button>
					<Link to={ROUTES.BIKES}>Bikes</Link>
					<Link to={ROUTES.GEAR}>Gear</Link>
					<Link to={ROUTES.ACCESSORIES}>Accessories</Link>
				</div>
				<SiteBrand className="mx-auto" />
				<div className="w-fit ml-auto flex items-center gap-5">
					<UserMenu />
				</div>
			</div>
		</header>
	);
}

function UserMenu() {
	const isAuthenticated = useStore((state) => state.isAuthenticated);

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
	return (
		<>
			<NavCart />
			<NavUser />
		</>
	);
}
