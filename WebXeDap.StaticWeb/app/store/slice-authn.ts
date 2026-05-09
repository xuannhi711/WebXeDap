import type { StateCreator } from "zustand";

type AuthnState = {
	accessToken?: string;
	refreshToken?: string;
	isAuthenticated: boolean;
};

type AuthnActions = {
	setToken: (accessToken: string, refreshToken?: string) => void;
	setAuthenticated: (isAuthenticated: boolean) => void;
	clearAuthnState: () => void;
};

export type AuthnSlice = AuthnState & AuthnActions;

const INIT_STATE: AuthnState = {
	accessToken: undefined,
	refreshToken: undefined,
	isAuthenticated: false,
};

export const createAuthnSlice: StateCreator<AuthnSlice> = (set) => ({
	...INIT_STATE,
	setToken: (accessToken, refreshToken) => set({ accessToken, refreshToken }),
	setAuthenticated: (isAuthenticated) => set({ isAuthenticated }),
	clearAuthnState: () => set(INIT_STATE),
});
