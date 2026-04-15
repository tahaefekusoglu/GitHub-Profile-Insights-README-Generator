import { ALL_SECTIONS } from "@/lib/types";

interface Props {
  enabled: string[];
  onChange: (sections: string[]) => void;
}

export default function SectionToggle({ enabled, onChange }: Props) {
  function toggle(id: string) {
    if (enabled.includes(id)) {
      onChange(enabled.filter((s) => s !== id));
    } else {
      onChange([...enabled, id]);
    }
  }

  function reset() {
    onChange(ALL_SECTIONS.map((s) => s.id));
  }

  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
      <div className="flex items-center justify-between mb-3">
        <p className="font-semibold text-gray-800 dark:text-gray-100">Sections</p>
        <button
          onClick={reset}
          className="text-xs text-violet-500 hover:text-violet-700 dark:hover:text-violet-300 transition-colors"
        >
          Reset
        </button>
      </div>
      <div className="space-y-2">
        {ALL_SECTIONS.map((section) => {
          const isEnabled = enabled.includes(section.id);
          return (
            <label
              key={section.id}
              className="flex items-center gap-3 cursor-pointer group"
            >
              <div
                onClick={() => toggle(section.id)}
                className={`w-10 h-6 rounded-full transition-colors relative flex-shrink-0 ${
                  isEnabled ? "bg-violet-500" : "bg-gray-200 dark:bg-gray-700"
                }`}
              >
                <div
                  className={`absolute top-1 w-4 h-4 rounded-full bg-white shadow transition-transform ${
                    isEnabled ? "translate-x-5" : "translate-x-1"
                  }`}
                />
              </div>
              <span className="text-sm text-gray-700 dark:text-gray-300 group-hover:text-gray-900 dark:group-hover:text-gray-100 transition-colors">
                {section.label}
              </span>
            </label>
          );
        })}
      </div>
    </div>
  );
}
