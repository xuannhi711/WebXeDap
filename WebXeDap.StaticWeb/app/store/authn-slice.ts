import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

export interface AuthenticatedUser {
	id: number;
	email: string;
	firstName: string;
	lastName: string;
	avatar: string;
}

export interface AuthnState {
	user: AuthenticatedUser | null;
	accessToken: string | null;
}

const initialState: AuthnState = {
	user: null,
	accessToken: null,
};

const authnSlice = createSlice({
	name: "authn",
	initialState,
	reducers: {
		setAuthnState(_state, action: PayloadAction<AuthnState>) {
			return action.payload;
		},
		updateAuthenticatedUser(
			state,
			action: PayloadAction<Partial<AuthenticatedUser>>,
		) {
			if (!state.user) {
				return;
			}

			state.user = {
				...state.user,
				...action.payload,
			};
		},
		clearAuthnState(state) {
			state.user = null;
			state.accessToken = null;
		},
	},
});

export const { setAuthnState, updateAuthenticatedUser, clearAuthnState } =
	authnSlice.actions;

export const authnReducer = authnSlice.reducer;
