using System.Net;

namespace Updater.IntegradorCRM.Downloader;

public class GitHubDownloader
{
    public void DownloadBranchZip()
    {
        string url = "https://github.com/xcnoris/Integrador-Com-CRM-V6/archive/refs/heads/master.zip";
        string tempPath = Path.Combine(Path.GetTempPath(), "master.zip");

        using (WebClient client = new WebClient())
        {
            client.Headers.Add("User-Agent", "CSharp-App");
            client.DownloadFile(url, tempPath);
        }

        Console.WriteLine($"Branch baixada para {tempPath}");
    }
}
