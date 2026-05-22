import type { StateCreator } from "zustand";

interface UserState {
	id: number;
	email: string;
	fullName: string;
	avatar: string;
}

type UserActions = {
	setUser: (user: UserState) => void;
	patchUser: (userPatch: Partial<UserState>) => void;
	clearUser: () => void;
};

export type UserSlice = Partial<UserState> & UserActions;

export const createUserSlice: StateCreator<UserSlice> = (set) => ({
	setUser: (user) => set(user),
	patchUser: (userPatch) => set((state) => ({ ...state, ...userPatch })),
	clearUser: () => set({}),
});
