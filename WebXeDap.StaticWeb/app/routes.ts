import { index, type RouteConfig, route } from "@react-router/dev/routes";

export default [index("routes/home.tsx")] satisfies RouteConfig;

export const ROUTES = {
	HOME: "/",
	LOGIN: "/login",
    REGISTER: "/register",
	FORGOT_PASSWORD: "/forgot-password",
} as const;
