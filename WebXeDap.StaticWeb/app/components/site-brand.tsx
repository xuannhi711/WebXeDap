import { Link } from "react-router";
import { ROUTES } from "~/routes";

interface SiteBrandProps {
	className?: string;
}

export function SiteBrand({ className }: SiteBrandProps) {
	return (
		<Link
			to={ROUTES.HOME}
			className={`text-2xl font-black flex items-center gap-1 w-fit ${className}`}
		>
			<img src="/favicon.svg" alt="favicon" className="h-10" />
			<h1>
				<em>WHEELIE</em>
			</h1>
		</Link>
	);
}
