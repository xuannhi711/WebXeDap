"use client";

import type { ComponentPropsWithoutRef } from "react";
import { Link } from "react-router";

import {
	SidebarGroup,
	SidebarGroupContent,
	SidebarMenu,
	SidebarMenuItem,
} from "~/components/ui/sidebar";
import { buttonVariants } from "../ui/button";

export function NavSecondary({
	items,
	...props
}: {
	items: {
		title: string;
		url: string;
		icon: React.ReactNode;
	}[];
} & ComponentPropsWithoutRef<typeof SidebarGroup>) {
	return (
		<SidebarGroup {...props}>
			<SidebarGroupContent>
				<SidebarMenu>
					{items.map((item) => (
						<SidebarMenuItem key={item.title}>
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
						</SidebarMenuItem>
					))}
				</SidebarMenu>
			</SidebarGroupContent>
		</SidebarGroup>
	);
}
