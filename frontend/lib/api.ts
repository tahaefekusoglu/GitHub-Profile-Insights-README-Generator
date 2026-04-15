import { GitHubProfile, ProfileAnalysis, ReadmeConfig } from "./types";

const BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:8080";

interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });
  const json: ApiResponse<T> = await res.json();
  if (!json.success || json.data === undefined) {
    throw new Error(json.error ?? "Unknown error");
  }
  return json.data;
}

export async function fetchGitHubProfile(username: string): Promise<GitHubProfile> {
  return request<GitHubProfile>(`/api/github/${encodeURIComponent(username)}`);
}

export async function generateBio(profile: GitHubProfile): Promise<string> {
  return request<string>("/api/bio/generate", {
    method: "POST",
    body: JSON.stringify(profile),
  });
}

export async function generateReadme(config: ReadmeConfig): Promise<string> {
  return request<string>("/api/readme/generate", {
    method: "POST",
    body: JSON.stringify(config),
  });
}

export async function fetchProfileAnalysis(username: string): Promise<ProfileAnalysis> {
  return request<ProfileAnalysis>(`/api/analysis/${encodeURIComponent(username)}`);
}
