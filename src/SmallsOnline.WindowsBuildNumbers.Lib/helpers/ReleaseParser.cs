using System.Text.RegularExpressions;
using SmallsOnline.WindowsBuildNumbers.Lib.Models;

namespace SmallsOnline.WindowsBuildNumbers.Lib.Helpers;

/// <summary>
/// Houses methods for parsing data from the Windows release history pages.
/// </summary>
public static class ReleaseParser
{
    private readonly static Regex _releaseRegex = new(
        pattern: @"<strong>Version (?'versionName'.{4})(?>\s\(RTM\)|) \(OS build (?'versionBuild'\d+)\)<\/strong>(?:(?:\n) - (?'isEoL'End of servicing)|)(?:(?s).+?)<table.+?>\n<tr>(?:\s*<th>.+?<\/th>){4}\n\s*<\/tr>(?'tableData'(?s).+?)\n<\/table>",
        options: RegexOptions.Compiled
    );

    private readonly static Regex _releaseTableRegex = new(
        pattern: @"<tr>\n<td>(?'servicingChannels'.+?)<\/td>\n<td>(?'releaseDate'\d{4}-\d{2}-\d{2})<\/td>\n<td>(?'buildNumber'.+?)<\/td>\n<td>(?:<a href=""(?'supportArticleUrl'.+?)"".+?>(?'kbArticleId'.+?)<\/a>|)<\/td>\n<\/tr>",
        options: RegexOptions.Compiled
    );

    private readonly static Regex _servicingTableSeparatorRegex = new(
        pattern: @"\s<span> (?:\u2022|(?:&bull;|(?:&#x2022;|&#8226;))) <\/span>\s",
        options: RegexOptions.Compiled
    );

    /// <summary>
    /// Parse the release info page's contents for Windows 10/11.
    /// </summary>
    /// <param name="releasePageContent">The content of the release info page.</param>
    /// <returns>A collection of regex matches.</returns>
    /// <exception cref="Exception"></exception>
    public static MatchCollection ParseReleaseInfoContent(string releasePageContent)
    {
        // Get all the matches for each feature update.
        MatchCollection releaseMatches = _releaseRegex.Matches(releasePageContent);

        // If there are no matches, throw an exception.
        if (releaseMatches.Count == 0)
        {
            throw new Exception("No releases found in the release page content.");
        }

        return releaseMatches;
    }

    /// <summary>
    /// Parse release info from a Windows feature update release match.
    /// </summary>
    /// <param name="releaseMatch">The feature update release match.</param>
    /// <param name="consumerLifecycleInfo">The consumer lifecycle info for the release.</param>
    /// <param name="enterpriseLifecycleInfo">The enterprise lifecycle info for the release.</param>
    /// <returns>A <see cref="ReleaseInfo" /> item.</returns>
    public static ReleaseInfo ParseReleaseInfo(Match releaseMatch, ReleaseLifecycleInfo consumerLifecycleInfo, ReleaseLifecycleInfo enterpriseLifecycleInfo)
    {
        // Return the release info from provided inputs.
        return new(
            releaseName: releaseMatch.Groups["versionName"].Value,
            consumerEoLDate: consumerLifecycleInfo.EndOfLifeDate,
            enterpriseEoLDate: enterpriseLifecycleInfo.EndOfLifeDate
        );
    }

    /// <summary>
    /// Parse the quality update release table data from a Windows feature update release match.
    /// </summary>
    /// <param name="releaseMatch">The feature update release match.</param>
    /// <returns>A list of <see cref="ReleaseBuild" /> items.</returns>
    public static IEnumerable<ReleaseBuild> ParseReleaseInfoBuilds(Match releaseMatch)
    {
        // Initialize a list to hold the parsed release builds.
        List<ReleaseBuild> releaseBuilds = new();

        // Parse the table data from the release match.
        MatchCollection releaseTableMatches = _releaseTableRegex.Matches(releaseMatch.Groups["tableData"].Value);

        // Loop through each table row match.
        foreach (Match releaseTableMatch in releaseTableMatches.AsEnumerable())
        {
            // Initialize a null support article URL.
            // Typically if there is not a support article URL,
            // it usually means that it's the initial release of the feature update.
            Uri? supportArticleUrl = null;

            // If a support article URL is found, then set supportArticleUrl to the parsed URL.
            if (releaseTableMatch.Groups["supportArticleUrl"].Success)
            {
                supportArticleUrl = new(releaseTableMatch.Groups["supportArticleUrl"].Value);
            }

            // Add the release build to the list with the parsed data.
            releaseBuilds.Add(
                new(
                    buildNumber: releaseTableMatch.Groups["buildNumber"].Value,
                    servicingChannels: SplitServicingChannels(releaseTableMatch.Groups["servicingChannels"].Value),
                    releaseDate: DateTimeOffset.Parse($"{releaseTableMatch.Groups["releaseDate"].Value} 18:00 -0:00"),
                    kbArticleId: releaseTableMatch.Groups["kbArticleId"].Value,
                    kbArticleUrl: supportArticleUrl
                )
            );
        }

        return releaseBuilds;
    }

    /// <summary>
    /// Split, if possible, the servicing channels from a quality update's servicing channels column.
    /// </summary>
    /// <param name="inputString">The captured string of the servicing channel table column.</param>
    /// <returns>A list of the servicing channels for the quality update.</returns>
    private static string[] SplitServicingChannels(string inputString)
    {
        string[] servicingChannels;

        // If the input string contains the servicing table separator, then split the string.
        if (_servicingTableSeparatorRegex.IsMatch(inputString))
        {
            // Split the string by the separator and normalize the servicing channel name.
            // Normalizing it helps with consistency since the older releases use acronyms
            // for the servicing channel names (Eg. CB -> Current Branch or CBB -> Current Branch for Business).
            servicingChannels = NormalizeServicingChannelString(_servicingTableSeparatorRegex.Split(inputString));
        }
        // Otherwise just normalize the string and add it to an array.
        else
        {
            servicingChannels = new[] { NormalizeServicingChannelString(inputString) };
        }

        return servicingChannels;
    }

    /// <summary>
    /// Normalize the servicing channel string, if needed.
    /// </summary>
    /// <param name="servicingChannel">The input servicing channel to normalize.</param>
    /// <returns>A normalized string of the servicing channel.</returns>
    /// <exception cref="ArgumentException"></exception>
    private static string NormalizeServicingChannelString(string servicingChannel) => servicingChannel switch
    {
        "LTSB" => "Long-Term Servicing Branch",
        "LTSC" => "Long-Term Servicing Channel",
        "CB" => "Current Branch",
        "CBB" => "Current Branch for Business",
        "Semi-Annual Channel" or "Semi-Annual Channel (Targeted)" or "General Availability Channel" => servicingChannel,
        _ => throw new ArgumentException($"Unknown servicing channel: {servicingChannel}")
    };

    /// <summary>
    /// Normalize the servicing channel strings, if needed.
    /// </summary>
    /// <param name="servicingChannels">The input servicing channels to normalize.</param>
    /// <returns>Normalized strings of the servicing channels.</returns>
    private static string[] NormalizeServicingChannelString(string[] servicingChannels)
    {
        for (int i = 0; i < servicingChannels.Length; i++)
        {
            servicingChannels[i] = NormalizeServicingChannelString(servicingChannels[i]);
        }

        return servicingChannels;
    }
}