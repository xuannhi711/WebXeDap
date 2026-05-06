import { Eye, EyeClosed } from "lucide-react";
import { useState } from "react";
import { InputGroup, InputGroupAddon, InputGroupInput } from "./input-group";

export const InputPassword = ({
	className,
	...props
}: React.ComponentProps<"input">) => {
	const [showPassword, setShowPassword] = useState(false);

	return (
		<InputGroup>
			<InputGroupInput
				type={showPassword ? "text" : "password"}
				placeholder="*******"
				{...props}
			/>
			<InputGroupAddon align="inline-end">
				<button
					className="mr-2"
					type="button"
					onClick={() => setShowPassword(!showPassword)}
				>
					{showPassword ? <Eye /> : <EyeClosed />}
				</button>
			</InputGroupAddon>
		</InputGroup>
	);
};
