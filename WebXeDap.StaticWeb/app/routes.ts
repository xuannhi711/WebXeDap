import { index, type RouteConfig, route } from "@react-router/dev/routes";

export default [
	index("routes/home.tsx"),
	route("login", "routes/login.tsx"),
	route("register", "routes/resgister.tsx"),
] satisfies RouteConfig;

export const ROUTES = {
	HOME: "/",
	LOGIN: "/login",
    REGISTER: "/register",
	FORGOT_PASSWORD: "/forgot-password",
} as const;
