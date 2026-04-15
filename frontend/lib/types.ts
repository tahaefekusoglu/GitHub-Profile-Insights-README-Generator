export interface GitHubRepo {
  name: string;
  description: string | null;
  htmlUrl: string;
  stars: number;
  forks: number;
  language: string | null;
}

export interface LanguageStat {
  language: string;
  percentage: number;
}

export interface GitHubProfile {
  login: string;
  name: string;
  bio: string | null;
  avatarUrl: string;
  location: string | null;
  company: string | null;
  blog: string | null;
  publicRepos: number;
  followers: number;
  following: number;
  totalStars: number;
  createdAt: string;
  topRepos: GitHubRepo[];
  topLanguages: LanguageStat[];
}

export interface ReadmeConfig {
  username: string;
  profile: GitHubProfile;
  theme: "minimal" | "colorful" | "dark";
  enabledSections: string[];
  aiBio: string | null;
}

export type Theme = "minimal" | "colorful" | "dark";

export interface AnalysisStats {
  publicRepos: number;
  totalStars: number;
  followers: number;
  following: number;
  yearsOnGitHub: number;
  avatarUrl: string;
  name: string | null;
  bio: string | null;
  location: string | null;
  company: string | null;
  blog: string | null;
  languages: LanguageStat[];
  topRepos: GitHubRepo[];
}

export interface ProfileAnalysis {
  username: string;
  developerType: string;
  experienceLevel: string;
  primaryFocus: string;
  strengths: string[];
  insights: string[];
  techStack: string[];
  summary: string;
  isAiGenerated: boolean;
  aiProvider: string | null;
  stats: AnalysisStats;
}

export interface AiProviderInfo {
  id: string;
  name: string;
  defaultModel: string;
}

export interface AvailableProvidersResponse {
  available: AiProviderInfo[];
}

// Provider id → default model id
export const PROVIDER_DEFAULTS: Record<string, string> = {
  claude: "claude-3-5-sonnet-20241022",
  openai: "gpt-4o-mini",
  gemini: "gemini-1.5-flash",
};

export const ALL_SECTIONS = [
  { id: "header", label: "Header" },
  { id: "about", label: "About Me" },
  { id: "ai_bio", label: "AI Bio" },
  { id: "stats", label: "GitHub Stats" },
  { id: "languages", label: "Top Languages" },
  { id: "top_repos", label: "Top Repos" },
  { id: "socials", label: "Social Links" },
] as const;
