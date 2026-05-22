import type { StateCreator } from "zustand";

type CartState = {
	cartCount: number;
};

type CartActions = {
	setCartCount: (count: number) => void;
};

export type CartSlice = CartState & CartActions;

const INIT_STATE: CartState = {
	cartCount: 0,
};

export const createCartSlice: StateCreator<CartSlice> = (set) => ({
	...INIT_STATE,
	setCartCount: (count) => set({ cartCount: count }),
});
