"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { AlertCircle } from "lucide-react";
import { fetchAvailableProviders, fetchProfileAnalysis } from "@/lib/api";
import { AiProviderInfo, ProfileAnalysis } from "@/lib/types";
import AiModelSelector from "@/components/AiModelSelector";

const EXPERIENCE_COLORS: Record<string, string> = {
  Junior: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200",
  "Mid-level": "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200",
  Senior: "bg-violet-100 text-violet-800 dark:bg-violet-900 dark:text-violet-200",
  Expert: "bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200",
};

const LANG_COLORS = [
  "bg-blue-500", "bg-violet-500", "bg-green-500", "bg-orange-500",
  "bg-pink-500", "bg-cyan-500", "bg-yellow-500", "bg-red-500",
];

function StatCard({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-3 text-center">
      <p className="text-xl font-bold text-gray-900 dark:text-gray-100">{value}</p>
      <p className="text-xs text-gray-500 dark:text-gray-400 mt-1 leading-tight">{label}</p>
    </div>
  );
}

function AnalysisSkeleton() {
  return (
    <div className="space-y-6 animate-pulse">
      <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-6">
        <div className="flex items-center gap-4 mb-4">
          <div className="w-20 h-20 rounded-full bg-gray-200 dark:bg-gray-700" />
          <div className="space-y-2 flex-1">
            <div className="h-5 w-40 bg-gray-200 dark:bg-gray-700 rounded" />
            <div className="h-4 w-28 bg-gray-200 dark:bg-gray-700 rounded" />
          </div>
        </div>
        <div className="grid grid-cols-4 gap-3">
          {[...Array(4)].map((_, i) => (
            <div key={i} className="h-16 bg-gray-100 dark:bg-gray-800 rounded-xl" />
          ))}
        </div>
      </div>
      <div className="h-32 bg-gray-100 dark:bg-gray-800 rounded-xl" />
    </div>
  );
}

function ProviderBadge({ provider }: { provider: string | null }) {
  if (provider === "claude")
    return <span className="px-2.5 py-1 rounded-full text-xs font-semibold bg-violet-100 text-violet-700 dark:bg-violet-900 dark:text-violet-300 border border-violet-200 dark:border-violet-800">✨ Claude</span>;
  if (provider === "openai")
    return <span className="px-2.5 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300 border border-green-200 dark:border-green-800">✨ GPT</span>;
  if (provider === "gemini")
    return <span className="px-2.5 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300 border border-blue-200 dark:border-blue-800">✨ Gemini</span>;
  return <span className="px-2.5 py-1 rounded-full text-xs font-semibold bg-gray-100 text-gray-600 dark:bg-gray-800 dark:text-gray-400 border border-gray-200 dark:border-gray-700">Algorithmic</span>;
}

export default function AnalyzePage({ params }: { params: { username: string } }) {
  const router = useRouter();
  const { username } = params;

  const [providers, setProviders] = useState<AiProviderInfo[]>([]);
  const [selectedProvider, setSelectedProvider] = useState("");
  const [customModel, setCustomModel] = useState("");

  const [analysis, setAnalysis] = useState<ProfileAnalysis | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    document.title = `${username}'s Analysis — GitHub Profile Tools`;
  }, [username]);

  useEffect(() => {
    fetchAvailableProviders()
      .then((res) => setProviders(res.available))
      .catch(() => {});
  }, []);

  useEffect(() => {
    runAnalysis("", "");
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [username]);

  function runAnalysis(provider: string, model: string) {
    setLoading(true);
    setError(null);
    setAnalysis(null);
    fetchProfileAnalysis(username, provider || undefined, model.trim() || undefined)
      .then(setAnalysis)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
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
          <button onClick={() => router.push("/")} className="px-5 py-2 rounded-xl bg-blue-600 hover:bg-blue-700 text-white font-semibold text-sm">
            Try another username
          </button>
        </div>
      </main>
    );
  }

  const stats = analysis?.stats;

  return (
    <main className="min-h-screen bg-gray-50 dark:bg-gray-950 p-4 md:p-8">
      <div className="max-w-3xl mx-auto">
        <div className="mb-4">
          <button onClick={() => router.push("/")} className="text-sm text-gray-500 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-100 transition-colors">
            ← Back
          </button>
          <div className="flex items-center gap-3 mt-2 flex-wrap">
            <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">Profile Analysis</h1>
            {!loading && analysis && <ProviderBadge provider={analysis.aiProvider} />}
          </div>
        </div>

        {/* AI selector + re-analyze */}
        <div className="mb-6 space-y-3">
          <AiModelSelector
            providers={providers}
            provider={selectedProvider}
            model={customModel}
            onProviderChange={setSelectedProvider}
            onModelChange={setCustomModel}
          />
          <button
            onClick={() => runAnalysis(selectedProvider, customModel)}
            disabled={loading}
            className="w-full py-2.5 rounded-xl bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-semibold text-sm transition-colors flex items-center justify-center gap-2"
          >
            {loading ? (
              <>
                <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                Analyzing...
              </>
            ) : "↺ Re-analyze"}
          </button>
        </div>

        {loading ? (
          <AnalysisSkeleton />
        ) : analysis && stats ? (
          <div className="space-y-6">
            {/* Profile Header */}
            <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-6">
              <div className="flex items-start gap-4 mb-5">
                {stats.avatarUrl && (
                  <Image src={stats.avatarUrl} alt={username} width={80} height={80} className="rounded-full border border-gray-200 dark:border-gray-700" />
                )}
                <div className="flex-1 min-w-0">
                  <div className="flex flex-wrap items-center gap-2 mb-1">
                    <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">{stats.name || username}</h2>
                    <span className="text-sm text-gray-500 dark:text-gray-400">@{username}</span>
                  </div>
                  <div className="flex flex-wrap gap-2 mb-2">
                    <span className="px-3 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200">{analysis.developerType}</span>
                    <span className={`px-3 py-1 rounded-full text-xs font-semibold ${EXPERIENCE_COLORS[analysis.experienceLevel] ?? "bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300"}`}>{analysis.experienceLevel}</span>
                  </div>
                  {stats.bio && <p className="text-sm text-gray-600 dark:text-gray-300">{stats.bio}</p>}
                  {stats.location && <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">{stats.location}{stats.company ? ` · ${stats.company}` : ""}</p>}
                </div>
              </div>
              <div className="grid grid-cols-3 sm:grid-cols-6 gap-2">
                <StatCard label="Public Repos" value={stats.publicRepos} />
                <StatCard label="Total Stars" value={stats.totalStars} />
                <StatCard label="Followers" value={stats.followers} />
                <StatCard label="Following" value={stats.following} />
                <StatCard label="Years on GitHub" value={stats.yearsOnGitHub} />
                <StatCard label="Avg Stars / Repo" value={stats.publicRepos > 0 ? Math.round(stats.totalStars / stats.publicRepos) : 0} />
              </div>
            </div>

            {/* Languages */}
            {stats.languages.length > 0 && (
              <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
                <h3 className="text-sm font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide mb-4">Most Used Languages</h3>
                <div className="flex h-4 rounded-full overflow-hidden mb-4 gap-0.5">
                  {stats.languages.map((lang, i) => (
                    <div key={lang.language} title={`${lang.language} ${lang.percentage}%`} className={`${LANG_COLORS[i % LANG_COLORS.length]} transition-all`} style={{ width: `${lang.percentage}%` }} />
                  ))}
                </div>
                <div className="space-y-2.5">
                  {stats.languages.map((lang, i) => (
                    <div key={lang.language} className="flex items-center gap-3">
                      <span className={`w-2.5 h-2.5 rounded-full shrink-0 ${LANG_COLORS[i % LANG_COLORS.length]}`} />
                      <span className="text-sm text-gray-700 dark:text-gray-300 w-28 shrink-0">{lang.language}</span>
                      <div className="flex-1 h-2 rounded-full bg-gray-100 dark:bg-gray-800 overflow-hidden">
                        <div className={`h-full rounded-full ${LANG_COLORS[i % LANG_COLORS.length]}`} style={{ width: `${lang.percentage}%` }} />
                      </div>
                      <span className="text-sm font-medium text-gray-600 dark:text-gray-400 w-10 text-right shrink-0">{lang.percentage}%</span>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Summary */}
            <div className="rounded-xl border border-blue-200 dark:border-blue-900 bg-blue-50 dark:bg-blue-950 p-5">
              <h3 className="text-sm font-semibold text-blue-800 dark:text-blue-200 uppercase tracking-wide mb-2">Summary</h3>
              <p className="text-gray-800 dark:text-gray-100 leading-relaxed">{analysis.summary}</p>
            </div>

            {/* Primary Focus */}
            <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
              <h3 className="text-sm font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide mb-2">Primary Focus</h3>
              <p className="text-gray-800 dark:text-gray-100">{analysis.primaryFocus}</p>
            </div>

            {/* Strengths + Insights */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="rounded-xl border border-green-200 dark:border-green-900 bg-white dark:bg-gray-900 p-5">
                <h3 className="text-sm font-semibold text-green-700 dark:text-green-400 uppercase tracking-wide mb-3">Strengths</h3>
                <ul className="space-y-2">
                  {analysis.strengths.map((s, i) => (
                    <li key={i} className="flex items-start gap-2 text-sm text-gray-700 dark:text-gray-300">
                      <span className="text-green-500 mt-0.5">✓</span>{s}
                    </li>
                  ))}
                </ul>
              </div>
              <div className="rounded-xl border border-amber-200 dark:border-amber-900 bg-white dark:bg-gray-900 p-5">
                <h3 className="text-sm font-semibold text-amber-700 dark:text-amber-400 uppercase tracking-wide mb-3">Insights</h3>
                <ul className="space-y-2">
                  {analysis.insights.map((s, i) => (
                    <li key={i} className="flex items-start gap-2 text-sm text-gray-700 dark:text-gray-300">
                      <span className="text-amber-500 mt-0.5">→</span>{s}
                    </li>
                  ))}
                </ul>
              </div>
            </div>

            {/* Tech Stack */}
            <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
              <h3 className="text-sm font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide mb-3">Tech Stack</h3>
              <div className="flex flex-wrap gap-2">
                {analysis.techStack.map((tech) => (
                  <span key={tech} className="px-3 py-1 rounded-full text-sm bg-gray-100 dark:bg-gray-800 text-gray-800 dark:text-gray-200 border border-gray-200 dark:border-gray-700">{tech}</span>
                ))}
              </div>
            </div>

            {/* Top Repos */}
            {stats.topRepos.length > 0 && (
              <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900 p-5">
                <h3 className="text-sm font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide mb-3">Notable Repositories</h3>
                <div className="space-y-3">
                  {stats.topRepos.slice(0, 5).map((repo) => (
                    <a key={repo.name} href={repo.htmlUrl} target="_blank" rel="noopener noreferrer" className="block p-3 rounded-lg border border-gray-100 dark:border-gray-800 hover:border-blue-300 dark:hover:border-blue-700 hover:bg-blue-50 dark:hover:bg-blue-950 transition-colors">
                      <div className="flex items-center justify-between gap-2">
                        <span className="font-medium text-sm text-gray-900 dark:text-gray-100">{repo.name}</span>
                        <div className="flex items-center gap-3 text-xs text-gray-500 dark:text-gray-400 shrink-0">
                          {repo.language && <span>{repo.language}</span>}
                          <span>★ {repo.stars}</span>
                        </div>
                      </div>
                      {repo.description && <p className="text-xs text-gray-500 dark:text-gray-400 mt-1 line-clamp-2">{repo.description}</p>}
                    </a>
                  ))}
                </div>
              </div>
            )}

            {/* CTA */}
            <div className="flex gap-3 pb-4">
              <button onClick={() => router.push(`/generate/${username}`)} className="flex-1 py-3 rounded-xl bg-violet-600 hover:bg-violet-700 text-white font-semibold text-sm transition-colors">
                Generate README for @{username}
              </button>
              <button onClick={() => router.push("/")} className="px-4 py-3 rounded-xl border border-gray-200 dark:border-gray-700 text-gray-600 dark:text-gray-300 text-sm font-medium hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors">
                ← Back
              </button>
            </div>
          </div>
        ) : null}
      </div>
    </main>
  );
}
