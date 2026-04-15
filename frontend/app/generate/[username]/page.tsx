"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { AlertCircle } from "lucide-react";
import { fetchGitHubProfile, generateBio, generateReadme } from "@/lib/api";
import { ALL_SECTIONS, GitHubProfile, ReadmeConfig, Theme } from "@/lib/types";
import ProfileCard from "@/components/ProfileCard";
import ThemeSelector from "@/components/ThemeSelector";
import SectionToggle from "@/components/SectionToggle";
import ReadmePreview from "@/components/ReadmePreview";
import CopyButton from "@/components/CopyButton";

function ProfileCardSkeleton() {
  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5 animate-pulse">
      <div className="flex items-center gap-4 mb-4">
        <div className="w-16 h-16 rounded-full bg-gray-200 dark:bg-gray-700" />
        <div className="space-y-2">
          <div className="h-4 w-32 bg-gray-200 dark:bg-gray-700 rounded" />
          <div className="h-3 w-20 bg-gray-200 dark:bg-gray-700 rounded" />
        </div>
      </div>
      <div className="grid grid-cols-2 gap-2">
        {[...Array(4)].map((_, i) => (
          <div key={i} className="h-12 bg-gray-100 dark:bg-gray-800 rounded-lg" />
        ))}
      </div>
    </div>
  );
}

export default function GeneratePage({ params }: { params: { username: string } }) {
  const router = useRouter();
  const { username } = params;

  const [profile, setProfile] = useState<GitHubProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [theme, setTheme] = useState<Theme>("colorful");

  const [enabledSections, setEnabledSections] = useState<string[]>(
    ALL_SECTIONS.map((s) => s.id)
  );
  const [aiBio, setAiBio] = useState<string | null>(null);
  const [bioLoading, setBioLoading] = useState(false);
  const [bioError, setBioError] = useState<string | null>(null);
  const [readme, setReadme] = useState<string | null>(null);
  const [readmeLoading, setReadmeLoading] = useState(false);
  const [readmeError, setReadmeError] = useState<string | null>(null);

  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [activeTab, setActiveTab] = useState<"configure" | "preview">("configure");

  useEffect(() => {
    document.title = `${username}'s README — GitHub README Generator`;
  }, [username]);

  useEffect(() => {
    fetchGitHubProfile(username)
      .then(setProfile)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, [username]);

  useEffect(() => {
    const saved = localStorage.getItem("readme-theme") as Theme | null;
    if (saved === "minimal" || saved === "colorful" || saved === "dark") {
      setTheme(saved);
    }
  }, []);

  function handleThemeChange(t: Theme) {
    setTheme(t);
    localStorage.setItem("readme-theme", t);
    scheduleRebuild(t, enabledSections);
  }

  function handleSectionsChange(sections: string[]) {
    setEnabledSections(sections);
    scheduleRebuild(theme, sections);
  }

  function scheduleRebuild(t: Theme, sections: string[]) {
    if (!readme || !profile) return;
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(() => {
      const config: ReadmeConfig = {
        username,
        profile,
        theme: t,
        enabledSections: sections,
        aiBio,
      };
      generateReadme(config).then(setReadme).catch((err: Error) => {
        setReadmeError(err.message);
      });
    }, 300);
  }

  async function handleGenerateBio() {
    if (!profile) return;
    setBioLoading(true);
    setBioError(null);
    try {
      const bio = await generateBio(profile);
      setAiBio(bio);
      if (!enabledSections.includes("ai_bio")) {
        setEnabledSections((prev) => [...prev, "ai_bio"]);
      }
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : "Failed to generate bio";
      if (msg.toLowerCase().includes("not configured")) {
        setBioError("AI bio is not available — ANTHROPIC_API_KEY is not set on the server.");
      } else {
        setBioError(msg);
      }
    } finally {
      setBioLoading(false);
    }
  }

  async function handleBuildReadme() {
    if (!profile) return;
    setReadmeLoading(true);
    setReadmeError(null);
    try {
      const config: ReadmeConfig = {
        username,
        profile,
        theme,
        enabledSections,
        aiBio,
      };
      const md = await generateReadme(config);
      setReadme(md);
      setActiveTab("preview");
    } catch (err: unknown) {
      setReadmeError(err instanceof Error ? err.message : "Failed to generate README");
    } finally {
      setReadmeLoading(false);
    }
  }

  if (error) {
    const notFound = error.toLowerCase().includes("not found");
    return (
      <main className="flex min-h-screen items-center justify-center p-6">
        <div className="max-w-md w-full border-2 border-red-200 dark:border-red-900 rounded-xl p-8 text-center">
          <AlertCircle className="mx-auto mb-4 text-red-500" size={40} />
          <p className="text-gray-800 dark:text-gray-100 font-semibold mb-2">
            {notFound ? `No GitHub user found for "${username}"` : "Something went wrong"}
          </p>
          <p className="text-sm text-gray-500 dark:text-gray-400 mb-6">{error}</p>
          <button
            onClick={() => router.push("/")}
            className="px-5 py-2 rounded-xl bg-violet-600 hover:bg-violet-700 text-white font-semibold text-sm"
          >
            Try another username
          </button>
        </div>
      </main>
    );
  }

  const controls = (
    <div className="space-y-4">
      {loading ? <ProfileCardSkeleton /> : profile && <ProfileCard profile={profile} />}
      <ThemeSelector value={theme} onChange={handleThemeChange} />
      <SectionToggle enabled={enabledSections} onChange={handleSectionsChange} />

      {bioError && (
        <div className="rounded-xl border border-yellow-300 dark:border-yellow-700 bg-yellow-50 dark:bg-yellow-950 p-4 text-sm text-yellow-800 dark:text-yellow-200">
          {bioError}
        </div>
      )}

      <button
        onClick={handleGenerateBio}
        disabled={bioLoading || loading || !profile}
        className="w-full py-3 rounded-xl border border-violet-300 dark:border-violet-700 text-violet-700 dark:text-violet-300 font-semibold text-sm hover:bg-violet-50 dark:hover:bg-violet-950 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
      >
        {bioLoading ? (
          <>
            <span className="w-4 h-4 border-2 border-violet-400 border-t-transparent rounded-full animate-spin" />
            Generating Bio...
          </>
        ) : aiBio ? (
          "↺ Regenerate AI Bio"
        ) : (
          "✨ Generate AI Bio"
        )}
      </button>

      {aiBio && (
        <div className="rounded-xl border border-green-200 dark:border-green-800 bg-green-50 dark:bg-green-950 p-4 text-sm text-green-800 dark:text-green-200">
          <p className="font-semibold mb-1">AI Bio:</p>
          <p>{aiBio}</p>
        </div>
      )}

      {readmeError && (
        <div className="rounded-xl border border-red-200 dark:border-red-900 bg-red-50 dark:bg-red-950 p-4 text-sm text-red-700 dark:text-red-300">
          {readmeError}
        </div>
      )}

      <button
        onClick={handleBuildReadme}
        disabled={readmeLoading || loading || !profile}
        className="w-full py-3 rounded-xl bg-violet-600 hover:bg-violet-700 text-white font-semibold text-sm transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
      >
        {readmeLoading ? (
          <>
            <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
            Building README...
          </>
        ) : (
          "Build README"
        )}
      </button>
    </div>
  );

  const preview = (
    <div className="space-y-4">
      <ReadmePreview markdown={readme} />
      <CopyButton text={readme} />
    </div>
  );

  return (
    <>
      <main className="min-h-screen bg-gray-50 dark:bg-gray-950 p-4 md:p-8">
        <div className="max-w-6xl mx-auto">
          <div className="mb-6">
            <button
              onClick={() => router.push("/")}
              className="text-sm text-gray-500 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-100 transition-colors"
            >
              ← Back
            </button>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mt-2">
              @{username}
            </h1>
          </div>

          <div className="flex lg:hidden border border-gray-200 dark:border-gray-700 rounded-xl p-1 mb-6 bg-white dark:bg-gray-900">
            {(["configure", "preview"] as const).map((tab) => (
              <button
                key={tab}
                onClick={() => setActiveTab(tab)}
                className={`flex-1 py-2 rounded-lg text-sm font-medium capitalize transition-colors ${
                  activeTab === tab
                    ? "bg-violet-600 text-white"
                    : "text-gray-600 dark:text-gray-300 hover:text-gray-900 dark:hover:text-gray-100"
                }`}
              >
                {tab}
              </button>
            ))}
          </div>

          <div className="hidden lg:grid lg:grid-cols-2 lg:gap-8">
            {controls}
            {preview}
          </div>

          <div className="lg:hidden">
            {activeTab === "configure" ? controls : preview}
          </div>
        </div>
      </main>
    </>
  );
}
