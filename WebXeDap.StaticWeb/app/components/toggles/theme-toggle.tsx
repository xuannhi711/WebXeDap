import { Moon, Sun, SunMoon } from "lucide-react";
import { match } from "ts-pattern";
import { Button } from "~/components/ui/button";
import { useTheme } from "~/hooks/use-theme";

export function ThemeToggle() {
	const { mode, setMode, nextMode } = useTheme();

	function toggleModeHandler() {
		setMode(nextMode());
	}

	return (
		<Button
			type="button"
			variant="outline"
			className="rounded-full"
			onClick={toggleModeHandler}
		>
			{match(mode)
				.with("auto", () => <SunMoon />)
				.with("dark", () => <Moon />)
				.with("light", () => <Sun />)
				.exhaustive()}
		</Button>
	);
}
