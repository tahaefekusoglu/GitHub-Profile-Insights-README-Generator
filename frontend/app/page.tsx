"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

function extractUsername(input: string): string {
  const trimmed = input.trim();
  const match = trimmed.match(/(?:https?:\/\/)?github\.com\/([a-zA-Z0-9\-]+)/);
  if (match) return match[1];
  return trimmed;
}

export default function Home() {
  const router = useRouter();
  const [username, setUsername] = useState("");
  const [error, setError] = useState<string | null>(null);

  function navigate(mode: "generate" | "analyze") {
    const parsed = extractUsername(username);
    if (!parsed) {
      setError("Please enter a GitHub username or profile URL.");
      return;
    }
    setError(null);
    router.push(mode === "generate" ? `/generate/${parsed}` : `/analyze/${parsed}`);
  }

  function handleKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key === "Enter") navigate("generate");
  }

  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-6 bg-gradient-to-br from-violet-50 to-blue-50 dark:from-gray-950 dark:to-gray-900">
      <div className="w-full max-w-lg text-center">
        <h1 className="text-4xl font-bold mb-3 bg-gradient-to-r from-violet-600 to-blue-600 bg-clip-text text-transparent">
          GitHub Profile Tools
        </h1>
        <p className="text-gray-500 dark:text-gray-400 mb-8 text-lg">
          Enter a GitHub username or profile URL to get started.
        </p>

        <div className="mb-4">
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            onKeyDown={handleKeyDown}
            placeholder="tahaefekusoglu or github.com/tahaefekusoglu"
            className="w-full px-4 py-3 rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-violet-500 mb-3"
          />
          <div className="flex gap-3">
            <button
              onClick={() => navigate("generate")}
              className="flex-1 px-4 py-3 rounded-xl bg-violet-600 hover:bg-violet-700 text-white font-semibold transition-colors text-sm"
            >
              Generate README →
            </button>
            <button
              onClick={() => navigate("analyze")}
              className="flex-1 px-4 py-3 rounded-xl bg-blue-600 hover:bg-blue-700 text-white font-semibold transition-colors text-sm"
            >
              Analyze Profile →
            </button>
          </div>
        </div>

        {error && (
          <p className="text-red-500 text-sm mb-4">{error}</p>
        )}

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-10">
          {[
            {
              title: "README Generator",
              desc: "Pick a theme, toggle sections, and get a beautiful profile README with AI bio.",
              color: "border-violet-200 dark:border-violet-900",
            },
            {
              title: "Profile Analyzer",
              desc: "Get an AI-powered breakdown of skills, experience, tech stack, and insights.",
              color: "border-blue-200 dark:border-blue-900",
            },
            {
              title: "3 Beautiful Themes",
              desc: "Minimal, Colorful, and Dark — pick your style for the README.",
              color: "border-gray-200 dark:border-gray-800",
            },
            {
              title: "Live GitHub Data",
              desc: "Pulls real stats, repos, and language data directly from GitHub's API.",
              color: "border-gray-200 dark:border-gray-800",
            },
          ].map((card) => (
            <div
              key={card.title}
              className={`p-4 rounded-xl border ${card.color} bg-white dark:bg-gray-900 text-left`}
            >
              <p className="font-semibold text-gray-800 dark:text-gray-100 mb-1">{card.title}</p>
              <p className="text-sm text-gray-500 dark:text-gray-400">{card.desc}</p>
            </div>
          ))}
        </div>
      </div>
    </main>
  );
}
