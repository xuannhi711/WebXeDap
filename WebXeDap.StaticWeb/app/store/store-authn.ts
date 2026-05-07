import { match, P } from "ts-pattern";
import { create } from "zustand";

export interface AuthenticatedUser {
	id: number;
	email: string;
	firstName: string;
	lastName: string;
	avatar: string;
}

type AuthnState = {
	user?: AuthenticatedUser;
	accessToken?: string;
};

type AuthnActions = {
	setAuthnState: (state: Required<AuthnState>) => void;
	updateAuthenticatedUser: (userPatch: Partial<AuthenticatedUser>) => void;
	clearAuthnState: () => void;
};

type AuthnStore = AuthnState & AuthnActions;

const INITIAL_AUTHN_STATE: AuthnState = {
	user: undefined,
	accessToken: undefined,
};

export const useAuthnStore = create<AuthnStore>((set) => ({
	...INITIAL_AUTHN_STATE,

	setAuthnState: (state) => set({ ...state }),
	updateAuthenticatedUser: (userPatch) =>
		set((state) => ({
			user: match(state.user)
				.with(P.nullish, () => undefined)
				.otherwise((user) => ({ ...user, ...userPatch })),
		})),
	clearAuthnState: () => set(INITIAL_AUTHN_STATE),
}));
