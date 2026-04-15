using System.Text;
using GitHubReadmeGenerator.API.Models;

namespace GitHubReadmeGenerator.API.Services;

public class ReadmeTemplateService : IReadmeTemplateService
{
    public string Generate(ReadmeConfig config)
    {
        var sections = new List<string>();

        foreach (var sectionId in config.EnabledSections)
        {
            var content = sectionId switch
            {
                "header" => BuildHeader(config),
                "about" => BuildAbout(config),
                "ai_bio" => BuildAiBio(config),
                "stats" => BuildStats(config),
                "languages" => BuildLanguages(config),
                "top_repos" => BuildTopRepos(config),
                "socials" => BuildSocials(config),
                _ => null
            };

            if (!string.IsNullOrWhiteSpace(content))
                sections.Add(content);
        }

        return string.Join("\n\n", sections);
    }

    private string GetThemeName(string theme) => theme switch
    {
        "minimal" => "default",
        "dark" => "dark",
        _ => "radical"
    };

    private static string EnsureAbsoluteUrl(string url) =>
        url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
        url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? url
            : $"https://{url}";

    private string GetBadgeColor(string theme) => theme switch
    {
        "minimal" => "181717",
        "dark" => "000000",
        _ => "7c3aed"
    };

    private string BuildHeader(ReadmeConfig config)
    {
        var p = config.Profile;
        var sb = new StringBuilder();
        sb.AppendLine("<div align=\"center\">");
        sb.AppendLine();
        sb.AppendLine($"# Hi, I'm {p.Name} 👋");
        sb.AppendLine();

        if (config.Theme == "minimal")
        {
            sb.AppendLine("<hr />");
        }
        else
        {
            var color = config.Theme == "dark" ? "black&labelColor=black" : "blueviolet";
            sb.AppendLine($"![Profile Views](https://komarev.com/ghpvc/?username={p.Login}&color={color}&style=flat-square)");
        }

        sb.AppendLine();
        sb.Append("</div>");
        return sb.ToString();
    }

    private string BuildAbout(ReadmeConfig config)
    {
        var p = config.Profile;
        var sb = new StringBuilder();
        sb.AppendLine("## About Me");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(p.Location))
            sb.AppendLine($"- 📍 {p.Location}");
        if (!string.IsNullOrWhiteSpace(p.Company))
            sb.AppendLine($"- 🏢 {p.Company}");
        if (!string.IsNullOrWhiteSpace(p.Blog))
        {
            var blogUrl = EnsureAbsoluteUrl(p.Blog);
            sb.AppendLine($"- 🌐 [{p.Blog}]({blogUrl})");
        }

        sb.Append($"- 📅 GitHub member since {p.CreatedAt.Year}");
        return sb.ToString();
    }

    private string BuildAiBio(ReadmeConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.AiBio))
            return "";

        return $"## Bio\n\n> {config.AiBio}";
    }

    private string BuildStats(ReadmeConfig config)
    {
        var p = config.Profile;
        var theme = GetThemeName(config.Theme);
        return $"""
## GitHub Stats

<div align="center">

![GitHub Stats](https://github-readme-stats.vercel.app/api?username={p.Login}&theme={theme}&show_icons=true&hide_border=true&count_private=true)

</div>
""";
    }

    private string BuildLanguages(ReadmeConfig config)
    {
        var p = config.Profile;
        var theme = GetThemeName(config.Theme);
        return $"""
## Top Languages

<div align="center">

![Top Languages](https://github-readme-stats.vercel.app/api/top-langs/?username={p.Login}&theme={theme}&layout=compact&hide_border=true&langs_count=6)

</div>
""";
    }

    private string BuildTopRepos(ReadmeConfig config)
    {
        var p = config.Profile;
        var theme = GetThemeName(config.Theme);
        var repos = p.TopRepos.Take(4).ToList();
        if (repos.Count == 0)
            return "";

        var sb = new StringBuilder();
        sb.AppendLine("## Top Repositories");
        sb.AppendLine();
        sb.AppendLine("<div align=\"center\">");
        sb.AppendLine("<table>");

        for (int i = 0; i < repos.Count; i += 2)
        {
            sb.AppendLine("<tr>");
            for (int j = i; j < Math.Min(i + 2, repos.Count); j++)
            {
                var repo = repos[j];
                var cardUrl = $"https://github-readme-stats.vercel.app/api/pin/?username={p.Login}&repo={repo.Name}&theme={theme}&hide_border=true";
                sb.AppendLine("<td>");
                sb.AppendLine();
                sb.AppendLine($"[![{repo.Name}]({cardUrl})]({repo.HtmlUrl})");
                sb.AppendLine();
                sb.AppendLine("</td>");
            }

            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</table>");
        sb.Append("</div>");
        return sb.ToString();
    }

    private string BuildSocials(ReadmeConfig config)
    {
        var p = config.Profile;
        var color = GetBadgeColor(config.Theme);
        var sb = new StringBuilder();
        sb.AppendLine("## Connect With Me");
        sb.AppendLine();
        sb.AppendLine("<div align=\"center\">");
        sb.AppendLine();
        sb.AppendLine($"[![GitHub](https://img.shields.io/badge/GitHub-{p.Login}-{color}?style=for-the-badge&logo=github)](https://github.com/{p.Login})");

        if (!string.IsNullOrWhiteSpace(p.Blog))
        {
            var blogUrl = EnsureAbsoluteUrl(p.Blog);
            sb.AppendLine($"[![Website](https://img.shields.io/badge/Website-Visit-{color}?style=for-the-badge&logo=google-chrome)]({blogUrl})");
        }

        sb.AppendLine();
        sb.Append("</div>");
        return sb.ToString();
    }
}
