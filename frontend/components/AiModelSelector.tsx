"use client";

import { AiProviderInfo, PROVIDER_DEFAULTS } from "@/lib/types";

interface Props {
  providers: AiProviderInfo[];
  provider: string;
  model: string;
  onProviderChange: (p: string) => void;
  onModelChange: (m: string) => void;
}

const ALGORITHMIC = { id: "algorithmic", name: "Algorithmic (no AI)" };

export default function AiModelSelector({
  providers,
  provider,
  model,
  onProviderChange,
  onModelChange,
}: Props) {
  const allOptions = [ALGORITHMIC, ...providers];
  const selectedProvider = providers.find((p) => p.id === provider);
  const defaultModel = provider ? PROVIDER_DEFAULTS[provider] ?? "" : "";
  const placeholder = defaultModel ? `default: ${defaultModel}` : "";

  function handleProviderChange(id: string) {
    onProviderChange(id);
    onModelChange(""); // reset custom model on provider switch
  }

  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 p-4 space-y-3">
      <p className="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
        AI Provider & Model
      </p>

      {/* Provider dropdown */}
      <select
        value={provider || "algorithmic"}
        onChange={(e) => handleProviderChange(e.target.value === "algorithmic" ? "" : e.target.value)}
        className="w-full px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 text-sm text-gray-800 dark:text-gray-200 focus:outline-none focus:ring-2 focus:ring-blue-500"
      >
        {allOptions.map((p) => (
          <option key={p.id} value={p.id}>
            {p.name}
          </option>
        ))}
      </select>

      {/* Model input — only shown when a real provider is selected */}
      {selectedProvider && (
        <div>
          <input
            type="text"
            value={model}
            onChange={(e) => onModelChange(e.target.value)}
            placeholder={placeholder}
            className="w-full px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 text-sm text-gray-800 dark:text-gray-200 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">
            Enter any model ID — leave blank to use the default.
          </p>
        </div>
      )}
    </div>
  );
}
