import { configureStore } from "@reduxjs/toolkit";

import { authnReducer } from "~/store/authn-slice";

export const store = configureStore({
	reducer: {
		auth: authnReducer,
	},
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
