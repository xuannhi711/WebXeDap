"use client";
import { ChevronRightIcon } from "lucide-react";
import type { ComponentProps } from "react";
import { Link } from "react-router";
import {
	Collapsible,
	CollapsibleContent,
	CollapsibleTrigger,
} from "~/components/ui/collapsible";
import {
	SidebarGroup,
	SidebarGroupLabel,
	SidebarMenu,
	SidebarMenuAction,
	SidebarMenuItem,
	SidebarMenuSub,
	SidebarMenuSubItem
} from "~/components/ui/sidebar";
import { buttonVariants } from "../ui/button";

type NavCollapsibleItem = {
	title: string;
	url: string;
	icon: React.ReactNode;
	isActive?: boolean;
	items?: {
		title: string;
		url: string;
	}[];
};

interface NavCollapsibleProps {
	items: NavCollapsibleItem[];
}

export function NavCollapsible({
	items,
	className,
	...props
}: NavCollapsibleProps & ComponentProps<typeof SidebarGroup>) {
	return (
		<SidebarGroup className={className} {...props}>
			<SidebarGroupLabel>Platform</SidebarGroupLabel>
			<SidebarMenu>
				{items.map((item) => (
					<Collapsible
						key={item.title}
						defaultOpen={item.isActive}
						render={<SidebarMenuItem />}
					>
						<Link
							to={item.url}
							className={buttonVariants({
								variant: "ghost",
								className: "w-full justify-start",
							})}
						>
							{item.icon}
							<span>{item.title}</span>
						</Link>
						{item.items?.length ? (
							<>
								<SidebarMenuAction
									render={<CollapsibleTrigger />}
									className="aria-expanded:rotate-90"
								>
									<ChevronRightIcon />
									<span className="sr-only">Toggle</span>
								</SidebarMenuAction>
								<CollapsibleContent>
									<SidebarMenuSub>
										{item.items?.map((subItem) => (
											<SidebarMenuSubItem key={subItem.title}>
												<Link
													to={subItem.url}
													className={buttonVariants({
														variant: "ghost",
														className: "w-full justify-start",
													})}
												>
													<span>{subItem.title}</span>
												</Link>
											</SidebarMenuSubItem>
										))}
									</SidebarMenuSub>
								</CollapsibleContent>
							</>
						) : null}
					</Collapsible>
				))}
			</SidebarMenu>
		</SidebarGroup>
	);
}
