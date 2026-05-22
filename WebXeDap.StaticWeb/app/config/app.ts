export const THEME_LOCAL_STORAGE_KEY = "theme";
export const REQUEST_TIMEOUT = 5000;
export const API_HOST = "http://localhost:5279";

export const ENDPOINTS = {
	LOGIN: "/api/auth/login?useCookies=true",
	LOGIN_W_GGL: "/api/auth/login/google",
	REGISTER: "/api/auth/register",
	LOGOUT: "/api/users/logout",
	AUTH_INFO: "/api/auth/manage/info",
	// REFRESH_TOKEN: "/api/auth/refresh",
	ME: "/api/users/me",

	PRODUCTS: "/api/products",
};
