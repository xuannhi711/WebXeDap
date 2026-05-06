import {
	BookOpenIcon,
	BotIcon,
	FrameIcon,
	MapIcon,
	PieChartIcon,
	Settings2Icon,
	TerminalSquareIcon,
} from "lucide-react";
import type { ComponentProps } from "react";
import { NavCollapsible } from "~/components/sidebar-navs/nav-collapsible";
import { NavUser } from "~/components/sidebar-navs/nav-user";
import {
	Sidebar,
	SidebarContent,
	SidebarFooter,
	SidebarHeader,
} from "~/components/ui/sidebar";
import { NavSecondary } from "./sidebar-navs/nav-secondary";
import { SiteBrand } from "./site-brand";
import { ThemeToggle } from "./toggles/theme-toggle";
import { Link } from "react-router";
import { buttonVariants } from "./ui/button";
import { useAppSelector } from "~/store/hooks";
import { cn } from "~/lib/utils";

const data = {
	navMain: [
		{
			title: "Playground",
			url: "#",
			icon: <TerminalSquareIcon />,
			isActive: true,
			items: [
				{
					title: "History",
					url: "#",
				},
				{
					title: "Starred",
					url: "#",
				},
				{
					title: "Settings",
					url: "#",
				},
			],
		},
		{
			title: "Models",
			url: "#",
			icon: <BotIcon />,
			items: [
				{
					title: "Genesis",
					url: "#",
				},
				{
					title: "Explorer",
					url: "#",
				},
				{
					title: "Quantum",
					url: "#",
				},
			],
		},
		{
			title: "Documentation",
			url: "#",
			icon: <BookOpenIcon />,
			items: [
				{
					title: "Introduction",
					url: "#",
				},
				{
					title: "Get Started",
					url: "#",
				},
				{
					title: "Tutorials",
					url: "#",
				},
				{
					title: "Changelog",
					url: "#",
				},
			],
		},
		{
			title: "Settings",
			url: "#",
			icon: <Settings2Icon />,
			items: [
				{
					title: "General",
					url: "#",
				},
				{
					title: "Team",
					url: "#",
				},
				{
					title: "Billing",
					url: "#",
				},
				{
					title: "Limits",
					url: "#",
				},
			],
		},
	],
	projects: [
		{
			title: "Design Engineering",
			url: "#",
			icon: <FrameIcon />,
		},
		{
			title: "Sales & Marketing",
			url: "#",
			icon: <PieChartIcon />,
		},
		{
			title: "Travel",
			url: "#",
			icon: <MapIcon />,
		},
	],
};

export function AppSidebar({ ...props }: ComponentProps<typeof Sidebar>) {
	return (
		<Sidebar
			className="top-(--header-height) h-[calc(100svh-var(--header-height))]!"
			{...props}
		>
			<SidebarHeader>
				<div className="flex justify-between items-center">
					<SiteBrand />
					<ThemeToggle />
				</div>
			</SidebarHeader>
			<SidebarContent>
				<NavCollapsible items={data.navMain} />
				<NavSecondary items={data.projects} />
			</SidebarContent>
			<SidebarFooter>
				<UserMenu />
			</SidebarFooter>
		</Sidebar>
	);
}

const OUTLINE_BTN_CLASSES = buttonVariants({
	variant: "outline",
	size: "lg",
});
const DEFAULT_BTN_CLASSES = buttonVariants({
	variant: "default",
	size: "lg",
});

function UserMenu() {
	const user = useAppSelector((state) => state.auth.user);
	const isAuthenticated = !!user;

	if (!isAuthenticated) {
		return (
			<div className="flex flex-col items-center gap-2">
				<Link to="/login" className={cn(OUTLINE_BTN_CLASSES, "w-full")}>
					Login
				</Link>
				<Link to="/register" className={cn(DEFAULT_BTN_CLASSES, "w-full")}>
					Register
				</Link>
			</div>
		);
	}
	return <NavUser />;
}
