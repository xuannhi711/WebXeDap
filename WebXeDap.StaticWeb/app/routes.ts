import {
	index,
	type RouteConfig,
	route,
	prefix,
} from "@react-router/dev/routes";

export default [
	index("routes/home.tsx"),
	route("login", "routes/login.tsx"),
	route("register", "routes/resgister.tsx"),
	...prefix("account", [route("me", "routes/account/me.tsx")]),
] satisfies RouteConfig;

export const ROUTES = {
	HOME: "/",
	LOGIN: "/login",
	REGISTER: "/register",
	FORGOT_PASSWORD: "/forgot-password",

	BIKES: "/products?category=bikes",
	GEAR: "/products?category=gear",
	ACCESSORIES: "/products?category=accessories",
} as const;
