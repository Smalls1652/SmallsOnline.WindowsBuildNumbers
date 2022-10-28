using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using SmallsOnline.WindowsBuildNumbers.Lib.Models;

namespace SmallsOnline.WindowsBuildNumbers.Lib.Helpers;

/// <summary>
/// Houses methods for getting release information for Windows.
/// </summary>
public class ReleaseInfoGetter : IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _isDisposed = false;

    public ReleaseInfoGetter()
    {
        _httpClient = new();
    }

    public ReleaseInfoGetter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Get the release info for all feature updates of Windows 10.
    /// </summary>
    /// <returns>A list of <see cref="ReleaseInfo" /> items.</returns>
    public async Task<List<ReleaseInfo>> GetWindowsReleaseInfoAsync() => await GetWindowsReleaseInfoAsync(MajorWindowsVersion.Windows10);

    /// <summary>
    /// Get the release info for all feature updates of a specific major Windows version.
    /// </summary>
    /// <param name="windowsVersion">The major release version of Windows.</param>
    /// <returns>A list of <see cref="ReleaseInfo" /> items.</returns>
    /// <exception cref="HttpRequestException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<List<ReleaseInfo>> GetWindowsReleaseInfoAsync(MajorWindowsVersion windowsVersion = MajorWindowsVersion.Windows10)
    {
        // Get the consumer lifecycle info for the specified major Windows version.
        List<ReleaseLifecycleInfo> consumerReleaseLifecycleInfo = windowsVersion switch
        {
            // Windows 11
            MajorWindowsVersion.Windows11 => await GetWindows11ReleaseLifecycleInfoAsync(SkuType.Consumer),
            
            // Windows 10 (Default)
            _ => await GetWindows10ReleaseLifecycleInfoAsync(SkuType.Consumer)
        };

        // Get the enterprise lifecycle info for the specified major Windows version.
        List<ReleaseLifecycleInfo> enterpriseReleaseLifecycleInfo = windowsVersion switch
        {
            // Windows 11
            MajorWindowsVersion.Windows11 => await GetWindows11ReleaseLifecycleInfoAsync(SkuType.Enterprise),

            // Windows 10 (Default)
            _ => await GetWindows10ReleaseLifecycleInfoAsync(SkuType.Enterprise)
        };

        // Get the URL for the release info page for the specified major Windows version.
        Uri releaseInfoUrl = windowsVersion switch
        {
            // Windows 11
            MajorWindowsVersion.Windows11 => ReleaseConstants.Windows11ReleaseInfoUrl,

            // Windows 10 (Default)
            _ => ReleaseConstants.Windows10ReleaseInfoUrl
        };

        // Initialize the list of release info items.
        List<ReleaseInfo> releaseInfoItems = new();

        // Initialize the HttpRequestMessage for getting the release info page.
        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: releaseInfoUrl
        );

        // Get the release info page.
        using HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

        // Check if the response was successful.
        // If not, throw an exception.
        if (responseMessage.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException($"Request failed with status code: {responseMessage.StatusCode}");
        }

        // Get the response content.
        string responseContent = await responseMessage.Content.ReadAsStringAsync();

        // Parse the response content for feature updates' release info.
        MatchCollection releaseMatches = ReleaseParser.ParseReleaseInfoContent(responseContent);

        // Loop through the matches and add them to the list of release info items.
        foreach (Match releaseMatch in releaseMatches.ToList())
        {
            // Get the corresponding consumer and enterprise lifecycle info for the matched feature update release.
            ReleaseLifecycleInfo? consumerReleaseLifecycleInfoItem = consumerReleaseLifecycleInfo.FirstOrDefault(x => x.ReleaseName == releaseMatch.Groups["versionName"].Value);
            ReleaseLifecycleInfo? enterpriseReleaseLifecycleInfoItem = enterpriseReleaseLifecycleInfo.FirstOrDefault(x => x.ReleaseName == releaseMatch.Groups["versionName"].Value);

            // Check if the consumer lifecycle info item was found.
            // If not, throw an exception.
            if (consumerReleaseLifecycleInfoItem is null)
            {
                throw new Exception($"No consumer release lifecycle info found for release: {releaseMatch.Groups["versionName"].Value}");
            }

            // Check if the enterprise lifecycle info item was found.
            // If not, throw an exception.
            if (enterpriseReleaseLifecycleInfoItem is null)
            {
                throw new Exception($"No enterprise release lifecycle info found for release: {releaseMatch.Groups["versionName"].Value}");
            }

            // Generate the ReleaseInfo item with the matched release info and the corresponding consumer and enterprise lifecycle info.
            ReleaseInfo releaseInfoItem = ReleaseParser.ParseReleaseInfo(releaseMatch, consumerReleaseLifecycleInfoItem, enterpriseReleaseLifecycleInfoItem);

            // Parse the matched release info for quality update releases
            // and add them to the ReleaseBuilds property of the ReleaseInfo item.
            releaseInfoItem.ReleaseBuilds.AddRange(ReleaseParser.ParseReleaseInfoBuilds(releaseMatch));

            // Add the created ReleaseInfo item to the list of release info items.
            releaseInfoItems.Add(releaseInfoItem);
        }

        return releaseInfoItems;
    }

    /// <summary>
    /// Get the release lifecycle info for all feature updates of Windows 10's consumer releases.
    /// </summary>
    /// <returns>A list of <see cref="ReleaseLifecycleInfo" /> items.</returns>
    public async Task<List<ReleaseLifecycleInfo>> GetWindows10ReleaseLifecycleInfoAsync() => await GetWindows10ReleaseLifecycleInfoAsync(SkuType.Consumer);

    /// <summary>
    /// Get the release lifecycle info for all feature updates of Windows 10's consumer or enterprise releases.
    /// </summary>
    /// <param name="skuType">The SKU type to get lifecyle info for.</param>
    /// <returns>A list of <see cref="ReleaseLifecycleInfo" /> items.</returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<List<ReleaseLifecycleInfo>> GetWindows10ReleaseLifecycleInfoAsync(SkuType skuType = SkuType.Consumer)
    {
        // Get the URL for the release lifecycle info page for the specified SKU type.
        Uri requestUrl = skuType switch
        {
            // Enterprise
            SkuType.Enterprise => ReleaseConstants.Windows10EnterpriseLifeCycleUrl,

            // Consumer (Default)
            _ => ReleaseConstants.Windows10ConsumerLifeCycleUrl
        };

        // Initialize the HttpRequestMessage for getting the release lifecycle info page.
        using HttpRequestMessage requestMsg = new(
            method: HttpMethod.Get,
            requestUri: requestUrl
        );

        // Get the release lifecycle info page.
        using HttpResponseMessage responseMsg = await _httpClient.SendAsync(requestMsg);

        // Check if the response was successful.
        // If not, throw an exception.
        if (responseMsg.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException($"Request to '{requestUrl}' failed with status code '{responseMsg.StatusCode}'.");
        }

        // Get the response content.
        string responseContent = await responseMsg.Content.ReadAsStringAsync();

        // Parse the response content for release lifecycle info.
        MatchCollection releaseLifecycleMatches = ReleaseLifecycleParser.ParseReleaseLifecycleContent(responseContent);

        // Initialize the list of release lifecycle info items by parsing the matches.
        List<ReleaseLifecycleInfo> releaseLifecycleInfos = ReleaseLifecycleParser.ParseReleaseLifecycleTableData(releaseLifecycleMatches);

        return releaseLifecycleInfos;
    }

    /// <summary>
    /// Get the release lifecycle info for all feature updates of Windows 11's consumer releases.
    /// </summary>
    /// <returns>A list of <see cref="ReleaseLifecycleInfo" /> items.</returns>
    public async Task<List<ReleaseLifecycleInfo>> GetWindows11ReleaseLifecycleInfoAsync() => await GetWindows11ReleaseLifecycleInfoAsync(SkuType.Consumer);

    /// <summary>
    /// Get the release lifecycle info for all feature updates of Windows 11's consumer or enterprise releases.
    /// </summary>
    /// <param name="skuType">The SKU type to get lifecyle info for.</param>
    /// <returns>A list of <see cref="ReleaseLifecycleInfo" /> items.</returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<List<ReleaseLifecycleInfo>> GetWindows11ReleaseLifecycleInfoAsync(SkuType skuType = SkuType.Consumer)
    {
        // Get the URL for the release lifecycle info page for the specified SKU type.
        Uri requestUrl = skuType switch
        {
            // Enterprise
            SkuType.Enterprise => ReleaseConstants.Windows11EnterpriseLifeCycleUrl,

            // Consumer (Default)
            _ => ReleaseConstants.Windows11ConsumerLifeCycleUrl
        };

        // Initialize the HttpRequestMessage for getting the release lifecycle info page.
        using HttpRequestMessage requestMsg = new(
            method: HttpMethod.Get,
            requestUri: requestUrl
        );

        // Get the release lifecycle info page.
        using HttpResponseMessage responseMsg = await _httpClient.SendAsync(requestMsg);

        // Check if the response was successful.
        // If not, throw an exception.
        if (responseMsg.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException($"Request to '{requestUrl}' failed with status code '{responseMsg.StatusCode}'.");
        }

        // Get the response content.
        string responseContent = await responseMsg.Content.ReadAsStringAsync();

        // Parse the response content for release lifecycle info.
        MatchCollection releaseLifecycleMatches = ReleaseLifecycleParser.ParseReleaseLifecycleContent(responseContent);

        // Initialize the list of release lifecycle info items by parsing the matches.
        List<ReleaseLifecycleInfo> releaseLifecycleInfos = ReleaseLifecycleParser.ParseReleaseLifecycleTableData(releaseLifecycleMatches);

        return releaseLifecycleInfos;
    }

    /// <summary>
    /// Dispose <see cref="ReleaseInfoGetter" />.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()" />
    protected virtual void Dispose(bool disposing)
    {
        // Dispose resources if the object is not already disposed.
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources.
                _httpClient.Dispose();
            }

            // Set the "_isDisposed" flag to true.
            _isDisposed = true;
        }
    }
}