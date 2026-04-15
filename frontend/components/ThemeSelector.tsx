import { Theme } from "@/lib/types";

interface Props {
  value: Theme;
  onChange: (theme: Theme) => void;
}

const THEMES: { id: Theme; label: string; colors: string[] }[] = [
  { id: "minimal", label: "Minimal", colors: ["bg-gray-300", "bg-gray-500", "bg-gray-700"] },
  { id: "colorful", label: "Colorful", colors: ["bg-violet-400", "bg-pink-400", "bg-orange-400"] },
  { id: "dark", label: "Dark", colors: ["bg-gray-900", "bg-gray-700", "bg-gray-500"] },
];

export default function ThemeSelector({ value, onChange }: Props) {
  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
      <p className="font-semibold text-gray-800 dark:text-gray-100 mb-3">Theme</p>
      <div className="flex gap-3">
        {THEMES.map((theme) => (
          <button
            key={theme.id}
            onClick={() => onChange(theme.id)}
            className={`flex-1 rounded-lg border-2 p-3 transition-all ${
              value === theme.id
                ? "border-violet-500 bg-violet-50 dark:bg-violet-950"
                : "border-gray-200 dark:border-gray-700 hover:border-gray-300"
            }`}
          >
            <div className="flex gap-1 justify-center mb-2">
              {theme.colors.map((c, i) => (
                <div key={i} className={`h-3 w-3 rounded-full ${c}`} />
              ))}
            </div>
            <p className="text-xs font-medium text-gray-700 dark:text-gray-300">{theme.label}</p>
          </button>
        ))}
      </div>
    </div>
  );
}
