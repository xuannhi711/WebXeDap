import {
	index,
	prefix,
	type RouteConfig,
	route,
} from "@react-router/dev/routes";

export default [
	index("routes/home.tsx"),
	route("login", "routes/login.tsx"),
	route("register", "routes/resgister.tsx"),
	...prefix("account", [route("me", "routes/account/me.tsx")]),
	...prefix("products", [
		index("routes/products/index.tsx"),
		route(":productId", "routes/products/product.tsx"),
	]),
	route("cart", "routes/cart.tsx"),
	route("payments/:paymentId", "routes/payments.$paymentId.tsx"),
] satisfies RouteConfig;

export const ROUTES = {
	HOME: "/",
	LOGIN: "/login",
	REGISTER: "/register",
	FORGOT_PASSWORD: "/forgot-password",

	BIKES: "/products?category=bikes",
	GEAR: "/products?category=gear",
	ACCESSORIES: "/products?category=accessories",
	PAYMENT: (paymentId: number | string) => `/payments/${paymentId}`,
} as const;
