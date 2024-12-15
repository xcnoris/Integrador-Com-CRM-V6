using Newtonsoft.Json;

namespace Updater.IntegradorCRM.Updater;

public class GitHubUpdater
{
    private const string ApiBaseUrl = "https://api.github.com";
    private const string RepoOwner = "xcnoris";
    private const string RepoName = "Integrador-Com-CRM-V6";
    private const string BranchName = "master";

    public async Task<string> GetLatestCommitShaAsync()
    {
        string url = $"{ApiBaseUrl}/repos/{RepoOwner}/{RepoName}/branches/{BranchName}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agente", "CSharp-App");
            var response = await client.GetStringAsync(url);

            var branchInfo = JsonConvert.DeserializeObject<Commit>(response);
            return branchInfo.Sha;
        }
    }
}