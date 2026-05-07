import { useEffect, useState } from "react";
import { match, P } from "ts-pattern";
import { THEME_LOCAL_STORAGE_KEY } from "~/config/app";

export const THEME_MODES = ["light", "dark", "auto"] as const;

export type ThemeMode = (typeof THEME_MODES)[number];

function getInitialMode(): ThemeMode {
	if (typeof window === "undefined") {
		return "auto";
	}

	const stored = window.localStorage.getItem(THEME_LOCAL_STORAGE_KEY);

	return match(stored)
		.with(P.union(...THEME_MODES), (mode) => mode)
		.otherwise(() => "auto");
}

function applyThemeMode(mode: ThemeMode) {
	window.localStorage.setItem(THEME_LOCAL_STORAGE_KEY, mode);
	const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
	const resolved = mode === "auto" ? (prefersDark ? "dark" : "light") : mode;

	document.documentElement.classList.remove("light", "dark");
	document.documentElement.classList.add(resolved);

	match(mode)
		.with("auto", () => document.documentElement.removeAttribute("data-theme"))
		.otherwise((m) => document.documentElement.setAttribute("data-theme", m));

	document.documentElement.style.colorScheme = resolved;
}

export function useTheme() {
	const [mode, setMode] = useState<ThemeMode>(getInitialMode());

	useEffect(() => {
		applyThemeMode(mode);
		// if (mode !== "auto") {
		// 	return;
		// }

		// const media = window.matchMedia("(prefers-color-scheme: dark)");
		// const onChange = () => applyThemeMode("auto");

		// media.addEventListener("change", onChange);
		// return () => {
		// 	media.removeEventListener("change", onChange);
		// };
	}, [mode]);

	function nextMode() {
		const currIdx = THEME_MODES.indexOf(mode);
		return THEME_MODES[(currIdx + 1) % THEME_MODES.length];
	}

	return { mode, setMode, nextMode };
}
