# GitHub Profile Insights & README Generator

A full-stack web application with two tools built into one: analyze any GitHub developer profile with AI-powered insights, and generate a beautiful profile README — all from just a username or URL. No account required.

![CI](https://github.com/tahaefekusoglu/github-profile-insights/actions/workflows/ci.yml/badge.svg)

---

## Screenshots

### Landing Page
![Landing Page](docs/screenshots/landing.png)

### Profile Insights — Stats, Languages & Summary
![Profile Insights](docs/screenshots/analyze-top.png)

### Profile Insights — Full Analysis
![Profile Insights Full](docs/screenshots/analyze-full.png)

### README Generator
![README Generator](docs/screenshots/generate.png)

---

## Features

### Tool 1 — Profile Insights

Enter any GitHub username and get a detailed, structured analysis of their developer profile.

**What you see:**
- **Developer type** — automatically detected (Full-Stack, Backend, iOS, Data Scientist, DevOps Engineer, Systems Engineer, etc.)
- **Experience level** — Junior / Mid-level / Senior / Expert, scored from real data
- **Stats dashboard** — public repos, total stars, followers, following, years on GitHub, average stars per repo
- **Language breakdown** — stacked rainbow bar + individual percentage bars with color coding per language
- **Primary focus** — a paragraph describing what the developer mainly works on
- **Strengths** — up to 5 specific, data-backed strong points
- **Insights** — observations about activity patterns, community reputation, language specialization
- **Tech stack** — top detected technologies shown as tags
- **Notable repositories** — top repos with stars, language, description, clickable links to GitHub
- **AI provider badge** — shows whether analysis was done by Claude, GPT-4o, Gemini, or the built-in algorithm

**Works without any API key.** When no AI key is configured, the built-in `LocalProfileAnalyzer` runs rule-based analysis from GitHub data — developer type detection, weighted experience scoring, and pattern-based insights. AI providers give richer, more natural descriptions.

---

### Tool 2 — README Generator

Generate a ready-to-paste `README.md` for your GitHub profile page in seconds.

**What you get:**
- Real data pulled from the GitHub API — repos, stars, followers, languages
- **3 visual themes** to choose from:
  - **Minimal** — clean, light style
  - **Colorful** — gradient, high-contrast dark style
  - **Dark** — dark background with muted tones
- **7 toggleable sections** — turn each on or off individually:
  - Header (name, avatar, location, company, website)
  - About Me (bio, GitHub member since, follower stats)
  - AI Bio (optional AI-written 2-3 sentence bio)
  - GitHub Stats (stats card via github-readme-stats)
  - Top Languages (language card via github-readme-stats)
  - Top Repos (HTML table with pinned repo cards)
  - Social Links (blog/website)
- **Optional AI bio** — click one button and an AI writes a personalized bio from your actual profile data
- **Live markdown preview** — rendered in real time, updates automatically when you change theme or toggle sections
- **One-click copy** — copies the raw markdown to your clipboard

---

## How It Works

```
You enter a GitHub username or URL (e.g. github.com/torvalds or just "torvalds")
                        │
                        ▼
         Backend fetches data from GitHub API
         (profile info + all public repos + language stats)
                        │
              ┌─────────┴──────────┐
              ▼                    ▼
     Profile Insights         README Generator
              │                    │
     AI Provider selected    Template engine builds
     (or local algorithm)    markdown from your config
              │                    │
              ▼                    ▼
     Structured analysis      Copy-paste README.md
     with badges, charts,     ready for your GitHub
     repos, insights          profile page
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| Frontend | Next.js 14 App Router + TypeScript + Tailwind CSS |
| AI — Option 1 | Claude 3.5 Sonnet (Anthropic) |
| AI — Option 2 | GPT-4o mini (OpenAI) |
| AI — Option 3 | Gemini 1.5 Flash (Google) |
| AI — Fallback | LocalProfileAnalyzer (no external call, always works) |
| Data | GitHub REST API v3 |
| Caching | ASP.NET Core IMemoryCache (5 min profiles, 2 min 404s) |
| Rate limiting | ASP.NET Core sliding window — 20 requests/min per IP |
| Resilience | Microsoft.Extensions.Http.Resilience — 3 retries, 500ms backoff |
| Backend hosting | Railway (Docker) |
| Frontend hosting | Vercel |
| CI | GitHub Actions |

---

## Project Structure

```
github-profile-insights/
│
├── backend/
│   └── GitHubReadmeGenerator.API/
│       ├── Controllers/
│       │   ├── GitHubController.cs      GET /api/github/{username}
│       │   ├── AnalysisController.cs    GET /api/analysis/{username}
│       │   ├── BioController.cs         POST /api/bio/generate
│       │   └── ReadmeController.cs      POST /api/readme/generate
│       │
│       ├── Services/
│       │   ├── IAiService.cs            Shared AI interface
│       │   ├── ClaudeService.cs         Anthropic Claude implementation
│       │   ├── OpenAiService.cs         OpenAI GPT-4o mini implementation
│       │   ├── GeminiService.cs         Google Gemini 1.5 Flash (REST)
│       │   ├── LocalProfileAnalyzer.cs  Rule-based fallback (no API key needed)
│       │   ├── AiProviderFactory.cs     Selects active provider from config
│       │   ├── GitHubService.cs         GitHub API client with caching
│       │   └── ReadmeTemplateService.cs Markdown template engine (3 themes)
│       │
│       ├── Models/
│       │   ├── GitHubProfile.cs
│       │   ├── GitHubRepo.cs
│       │   ├── LanguageStat.cs
│       │   ├── ProfileAnalysis.cs
│       │   ├── ReadmeConfig.cs
│       │   └── ApiResponse.cs
│       │
│       ├── appsettings.json
│       ├── appsettings.Development.json   ← your API keys go here (gitignored)
│       ├── Dockerfile
│       └── railway.json
│
├── frontend/
│   ├── app/
│   │   ├── page.tsx                       Landing — enter username, pick tool
│   │   ├── analyze/[username]/page.tsx    Profile Insights page
│   │   └── generate/[username]/page.tsx   README Generator page
│   │
│   ├── components/
│   │   ├── ProfileCard.tsx
│   │   ├── ThemeSelector.tsx
│   │   ├── SectionToggle.tsx
│   │   ├── ReadmePreview.tsx
│   │   └── CopyButton.tsx
│   │
│   ├── lib/
│   │   ├── api.ts       Typed API client functions
│   │   └── types.ts     TypeScript interfaces
│   │
│   └── vercel.json
│
├── docs/
│   └── screenshots/
│       ├── landing.png
│       ├── analyze-top.png
│       ├── analyze-full.png
│       └── generate.png
│
├── .github/
│   └── workflows/ci.yml    Build checks on every push
│
├── .gitignore
└── README.md
```

---

## Prerequisites

You need two tools installed. That's it.

| Tool | Min. version | Check | Download |
|---|---|---|---|
| .NET SDK | 8.x | `dotnet --version` | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Node.js | 18.x | `node --version` | [nodejs.org](https://nodejs.org) |

Both commands should return a version number. If they don't, install the tool first.

---

## Running Locally

### Step 1 — Clone the repo

```bash
git clone https://github.com/tahaefekusoglu/github-profile-insights.git
cd github-profile-insights
```

### Step 2 — Configure API keys (optional)

Create or open the file:
```
backend/GitHubReadmeGenerator.API/appsettings.Development.json
```

Fill in any keys you have. **All fields are optional** — the app works without any of them.

```json
{
  "GitHub": {
    "Token": ""
  },
  "Anthropic": {
    "ApiKey": ""
  },
  "OpenAI": {
    "ApiKey": ""
  },
  "Gemini": {
    "ApiKey": ""
  }
}
```

> **Where to get keys:**
> - **GitHub Token** → github.com → Settings → Developer Settings → Personal Access Tokens → Classic → select `public_repo` scope
> - **Anthropic** → console.anthropic.com → API Keys
> - **OpenAI** → platform.openai.com → API Keys
> - **Gemini** → aistudio.google.com → Get API Key

> **Without keys:** Profile analysis runs in Algorithmic mode. README Generator works fully. Only the "Generate AI Bio" button will return an error.

### Step 3 — Start the backend

Open a terminal in the project root:

```bash
cd backend/GitHubReadmeGenerator.API
dotnet run
```

Wait for this line:
```
Now listening on: http://0.0.0.0:8080
```

The API is now running at `http://localhost:8080`.

You can verify it with:
```bash
curl http://localhost:8080/api/health
# → {"status":"ok"}
```

### Step 4 — Start the frontend

Open a **second terminal** in the project root:

```bash
cd frontend
npm install      # only needed on the first run
npm run dev
```

Wait for:
```
▲ Next.js 14
- Local: http://localhost:3000
```

### Step 5 — Open in browser

Go to [http://localhost:3000](http://localhost:3000)

Enter any GitHub username (e.g. `torvalds`, `gaearon`, `tahaefekusoglu`) or paste a full GitHub profile URL.

---

## Stopping the App

In each terminal, press `Ctrl + C`.

To start again later (after the first run):
```bash
# Terminal 1
cd backend/GitHubReadmeGenerator.API && dotnet run

# Terminal 2
cd frontend && npm run dev
```

No need to run `npm install` again.

---

## Environment Variables

### Backend

Configure in `appsettings.Development.json` locally, or as environment variables in production.

| Variable | Config key | Default | Purpose |
|---|---|---|---|
| `GITHUB_TOKEN` | `GitHub:Token` | — | Raises GitHub API rate limit from 60 to 5,000 req/hr |
| `ANTHROPIC_API_KEY` | `Anthropic:ApiKey` | — | Enables Claude 3.5 Sonnet |
| `OPENAI_API_KEY` | `OpenAI:ApiKey` | — | Enables GPT-4o mini |
| `GEMINI_API_KEY` | `Gemini:ApiKey` | — | Enables Gemini 1.5 Flash |
| `ALLOWED_ORIGINS` | `AllowedOrigins` | `http://localhost:3000` | Comma-separated CORS origins |
| `PORT` | — | `8080` | Backend listening port |

### Frontend

| Variable | Default | Purpose |
|---|---|---|
| `NEXT_PUBLIC_API_URL` | `http://localhost:8080` | Points frontend to the backend |

---

## AI Provider Selection

The backend automatically picks the first configured provider. Priority order:

```
1. ANTHROPIC_API_KEY set?  →  Claude 3.5 Sonnet      (best quality)
2. OPENAI_API_KEY set?     →  GPT-4o mini             (fast, cheap)
3. GEMINI_API_KEY set?     →  Gemini 1.5 Flash        (free tier available)
4. None configured?        →  LocalProfileAnalyzer    (always works, no cost)
```

The analysis page shows a badge indicating which mode is active:

| Badge | Color | Means |
|---|---|---|
| `✨ Claude` | Violet | Powered by Anthropic Claude |
| `✨ GPT-4o` | Green | Powered by OpenAI GPT-4o mini |
| `✨ Gemini` | Blue | Powered by Google Gemini |
| `Algorithmic` | Gray | No API key — built-in rule-based analysis |

---

## Algorithmic Analysis (How It Works Without AI)

When no API key is configured, `LocalProfileAnalyzer.cs` generates the full analysis from raw GitHub data alone.

**Developer type** is detected by language distribution patterns:

| Language pattern | Detected type |
|---|---|
| Swift or Objective-C as primary | iOS Developer |
| Dart present | Flutter / Mobile Developer |
| Kotlin as #1 language | Android Developer |
| R, Julia, or MATLAB | Data Scientist |
| Python primary, no JavaScript | Python / Data Developer |
| Rust or Zig, no web languages | Systems Engineer |
| Shell/HCL only, no app languages | DevOps Engineer |
| JavaScript/TypeScript + backend language | Full-Stack Developer |
| JavaScript/TypeScript only | Frontend Developer |
| Java/Kotlin without web | JVM Developer |
| C# without web | .NET Developer |

**Experience level** uses a weighted score across four signals:

| Signal | Points |
|---|---|
| 10+ years on GitHub | 4 |
| 1,000+ total stars | 4 |
| 7–9 years | 3 |
| 300–999 stars | 3 |
| 500+ followers | 3 |
| 80+ public repos | 3 |
| 4–6 years | 2 |
| 50–299 stars | 2 |
| 40–79 repos | 2 |
| 100+ followers | 2 |
| 2–3 years | 1 |
| 10–49 stars | 1 |
| 15–39 repos | 1 |
| 20+ followers | 1 |

Score ≥ 10 → **Expert** · ≥ 6 → **Senior** · ≥ 3 → **Mid-level** · else → **Junior**

---

## API Reference

All endpoints return: `{ "success": boolean, "data": T, "error": string }`

### `GET /api/health`
Health check. Returns `{ "status": "ok" }`.

---

### `GET /api/github/{username}`
Fetches a GitHub profile with repos, stars, and language stats.

**Responses:**
| Status | Meaning |
|---|---|
| 200 | Profile data |
| 400 | Invalid username (must match `^[a-zA-Z0-9\-]{1,39}$`) |
| 404 | GitHub user not found |
| 429 | GitHub API rate limit exceeded |
| 500 | Unexpected error |

Results cached: 5 minutes for found profiles, 2 minutes for 404s.

---

### `GET /api/analysis/{username}`
Returns a full `ProfileAnalysis` object.

Never returns 503 — if no AI key is configured, falls back to algorithmic analysis automatically.

**Response fields:**
```json
{
  "username": "torvalds",
  "developerType": "Systems Engineer",
  "experienceLevel": "Expert",
  "primaryFocus": "...",
  "strengths": ["...", "..."],
  "insights": ["...", "..."],
  "techStack": ["C", "Shell", "Python"],
  "summary": "...",
  "isAiGenerated": false,
  "aiProvider": null,
  "stats": {
    "publicRepos": 10,
    "totalStars": 250,
    "followers": 230000,
    "following": 0,
    "yearsOnGitHub": 14,
    "languages": [...],
    "topRepos": [...]
  }
}
```

---

### `POST /api/bio/generate`
Generates a 2-3 sentence AI-written bio.

**Body:** `GitHubProfile` object (same shape as `/api/github/{username}` `data` field)

| Status | Meaning |
|---|---|
| 200 | Bio string |
| 503 | No AI provider configured |
| 500 | AI call failed |

---

### `POST /api/readme/generate`
Generates a full README markdown string.

**Body:**
```json
{
  "username": "torvalds",
  "theme": "colorful",
  "enabledSections": ["header", "about", "stats", "languages", "top_repos"],
  "aiBio": null,
  "profile": { }
}
```

Valid `theme` values: `minimal` · `colorful` · `dark`

Valid `enabledSections` values: `header` · `about` · `ai_bio` · `stats` · `languages` · `top_repos` · `socials`

| Status | Meaning |
|---|---|
| 200 | Markdown string |
| 400 | Invalid theme or no sections enabled |
| 500 | Template error |

---

## Deployment

### Backend → Railway

The repo includes a `Dockerfile` and `railway.json` for one-click Railway deployment.

1. Push your repo to GitHub
2. Go to [railway.app](https://railway.app) → New Project → Deploy from GitHub repo → select this repo
3. In Railway dashboard → Variables, add:
   - `ALLOWED_ORIGINS` = your Vercel frontend URL (e.g. `https://your-app.vercel.app`)
   - `GITHUB_TOKEN` = your GitHub token (recommended)
   - Any one of: `ANTHROPIC_API_KEY`, `OPENAI_API_KEY`, `GEMINI_API_KEY` (optional)
4. Railway builds and deploys automatically from the Dockerfile

---

### Frontend → Vercel

1. Go to [vercel.com](https://vercel.com) → New Project → Import your GitHub repo
2. Set **Root Directory** to `frontend`
3. Add environment variable:
   - `NEXT_PUBLIC_API_URL` = your Railway backend URL (e.g. `https://your-app.railway.app`)
4. Click Deploy

---

## CI/CD

Every push to the repository triggers `.github/workflows/ci.yml`:

- **build-backend** — `dotnet restore` + `dotnet build` (must pass with 0 errors)
- **build-frontend** — `npm ci` + `npm run build` (must pass with 0 errors)

---

## Security

| Protection | How |
|---|---|
| API keys never committed | `appsettings.Development.json` is in `.gitignore` |
| Production secrets isolated | Set only via environment variables |
| Input validation | Usernames validated against `^[a-zA-Z0-9\-]{1,39}$` before any external call |
| CORS locked down | Only configured origins are allowed |
| Request size limit | Write endpoints capped at 100KB |
| Rate limiting | 20 requests/min per IP on all endpoints |
| Retry safety | GitHub API calls retry up to 3 times with backoff — won't flood on transient errors |

---

## License

MIT
