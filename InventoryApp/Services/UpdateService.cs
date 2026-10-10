using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InventoryApp.Services;

public sealed class UpdateService
{
    private const string LatestReleaseUrl = "https://api.github.com/repos/devrupeshgadkhe/MAUI-Project/releases/latest";
    private static readonly HttpClient Http = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("InventoryApp", AppInfo.Current.VersionString));
        return client;
    }

    public async Task CheckAndOfferUpdateAsync(Page page, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await Http.GetAsync(LatestReleaseUrl, cancellationToken);
            if (!response.IsSuccessStatusCode) return;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = json.RootElement;
            if (!root.TryGetProperty("tag_name", out var tagElement)) return;

            var latestTag = tagElement.GetString()?.Trim();
            var latestVersionText = latestTag?.TrimStart('v', 'V');
            var currentVersionText = AppInfo.Current.VersionString.TrimStart('v', 'V');
            if (!Version.TryParse(latestVersionText, out var latestVersion) ||
                !Version.TryParse(currentVersionText, out var currentVersion) ||
                latestVersion <= currentVersion)
                return;

            var assetName = DeviceInfo.Platform == DevicePlatform.WinUI
                ? $"InventoryApp-Setup-{latestTag}.exe"
                : DeviceInfo.Platform == DevicePlatform.Android
                    ? $"InventoryApp-Android-{latestTag}.apk"
                    : null;
            if (assetName is null || !root.TryGetProperty("assets", out var assets)) return;

            string? downloadUrl = null;
            foreach (var asset in assets.EnumerateArray())
            {
                if (asset.TryGetProperty("name", out var name) &&
                    string.Equals(name.GetString(), assetName, StringComparison.OrdinalIgnoreCase) &&
                    asset.TryGetProperty("browser_download_url", out var url))
                {
                    downloadUrl = url.GetString();
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(downloadUrl)) return;

            var answer = await MainThread.InvokeOnMainThreadAsync(() =>
                page.DisplayAlert("Update available",
                    $"Inventory App {latestVersion} is available. You are using {currentVersion}. Download and install the update?",
                    "Update", "Later"));
            if (!answer) return;

            var updatePath = Path.Combine(FileSystem.CacheDirectory, assetName);
            using var download = await Http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            download.EnsureSuccessStatusCode();
            await using (var file = File.Create(updatePath))
                await download.Content.CopyToAsync(file, cancellationToken);

#if WINDOWS
            Process.Start(new ProcessStartInfo(updatePath) { UseShellExecute = true });
#elif ANDROID
            await Launcher.Default.OpenAsync(new OpenFileRequest(
                "Install Inventory App update",
                new ReadOnlyFile(updatePath, "application/vnd.android.package-archive")));
#else
            await MainThread.InvokeOnMainThreadAsync(() =>
                page.DisplayAlert("Update downloaded", $"The update was saved to: {updatePath}", "OK"));
#endif
        }
        catch (OperationCanceledException)
        {
            // A page can disappear while the network request is running.
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
                await page.DisplayAlert("Update check", $"Could not check or start the update. {ex.Message}", "OK"));
        }
    }
}
