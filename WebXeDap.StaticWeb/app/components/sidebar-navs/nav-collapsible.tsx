/** biome-ignore-all lint/a11y/useAnchorContent: <cai nay cua shadcn> */
"use client";
import type { ComponentProps } from "react";
import { ChevronRightIcon } from "lucide-react";
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
	SidebarMenuButton,
	SidebarMenuItem,
	SidebarMenuSub,
	SidebarMenuSubButton,
	SidebarMenuSubItem,
} from "~/components/ui/sidebar";

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
						<SidebarMenuButton
							tooltip={item.title}
							render={<a href={item.url} />}
						>
							{item.icon}
							<span>{item.title}</span>
						</SidebarMenuButton>
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
												<SidebarMenuSubButton render={<a href={subItem.url} />}>
													<span>{subItem.title}</span>
												</SidebarMenuSubButton>
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
