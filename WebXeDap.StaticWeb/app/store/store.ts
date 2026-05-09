import { create } from "zustand";
import { createAuthnSlice, type AuthnSlice } from "./slice-authn";
import { createUserSlice, type UserSlice } from "./slice-user";

type UltimiteStore = AuthnSlice & UserSlice;

export const useStore = create<UltimiteStore>()((...a) => ({
	...createAuthnSlice(...a),
	...createUserSlice(...a),
}));
