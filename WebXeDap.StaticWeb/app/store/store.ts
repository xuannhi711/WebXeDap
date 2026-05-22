import { create } from "zustand";
import { createAuthnSlice, type AuthnSlice } from "./slice-authn";
import { createUserSlice, type UserSlice } from "./slice-user";
import { type CartSlice, createCartSlice } from "./slice-cart";

type UltimiteStore = AuthnSlice & UserSlice & CartSlice;

export const useStore = create<UltimiteStore>()((...a) => ({
	...createAuthnSlice(...a),
	...createUserSlice(...a),
	...createCartSlice(...a),
}));
