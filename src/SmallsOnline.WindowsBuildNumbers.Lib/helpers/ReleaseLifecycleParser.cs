using System.Text.RegularExpressions;
using SmallsOnline.WindowsBuildNumbers.Lib.Models;

namespace SmallsOnline.WindowsBuildNumbers.Lib.Helpers;

/// <summary>
/// Holds the logic to parse the release lifecycle information for Windows 10/11 releases.
/// </summary>
public static class ReleaseLifecycleParser
{
    private static readonly Regex _releaseLifecycleTableRegex = new(
        pattern: @"<section>\n\s+<h2.+?>Releases<\/h2>\n\s+(?'tableData'(?s)<table.+?>.+?<\/table>)",
        options: RegexOptions.NonBacktracking
    );
    private static readonly Regex _releaseLifecycleTableDataRegex = new(
        pattern: @"<tr>\n\s+<td>Version (?'versionNumber'.{4})<\/td>\n\s+<td .+?>\n\s+.*(?'startDate'\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}-\d{2}:\d{2}).+\n\s+<\/td>\n\s+<td .+?>\n\s+.*(?'endDate'\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}-\d{2}:\d{2}).*\n\s+<\/td>\n\s+<\/tr>",
        options: RegexOptions.NonBacktracking
    );

    /// <summary>
    /// Parse the release lifecycle information page's contents for Windows 10/11.
    /// </summary>
    /// <param name="releaseLifecyclePageContent">The content of the lifecycle page.</param>
    /// <returns>A collection of regex matches.</returns>
    /// <exception cref="Exception"></exception>
    public static MatchCollection ParseReleaseLifecycleContent(string releaseLifecyclePageContent)
    {
        // Get all the matches for each release lifecycle table.
        MatchCollection releaseLifecycleMatches = _releaseLifecycleTableRegex.Matches(releaseLifecyclePageContent);

        // If there are no matches, throw an exception.
        if (releaseLifecycleMatches.Count == 0)
        {
            throw new Exception("No release lifecycle data found in the release lifecycle page content.");
        }

        return releaseLifecycleMatches;
    }

    /// <summary>
    /// Parse the release lifecycle table data for Windows 10/11.
    /// </summary>
    /// <param name="releaseLifecycleMatch">The collection of matches to parse.</param>
    /// <returns>A list of <see cref="ReleaseLifecycleInfo" /> items.</returns>
    public static List<ReleaseLifecycleInfo> ParseReleaseLifecycleTableData(MatchCollection releaseLifecycleMatch)
    {
        // Initialize the list to hold ReleaseLifecycleInfo items.
        List<ReleaseLifecycleInfo> releaseLifecycleInfos = new();

        // Loop through each match.
        foreach (Match tableDataMatch in releaseLifecycleMatch.ToList())
        {
            // Get the table data.
            string tableData = tableDataMatch.Groups["tableData"].Value;

            // Get all the matches for each release lifecycle table data.
            MatchCollection releaseLifecycleTableDataMatches = _releaseLifecycleTableDataRegex.Matches(tableData);

            // Loop through each match from the table data.
            foreach (Match releaseLifecycleTableDataMatch in releaseLifecycleTableDataMatches.ToList())
            {
                // Add a new ReleaseLifecycleInfo item to the list with the data from the match.
                releaseLifecycleInfos.Add(
                    new(
                        releaseName: releaseLifecycleTableDataMatch.Groups["versionNumber"].Value,
                        endOfLifeDate: DateTimeOffset.Parse(releaseLifecycleTableDataMatch.Groups["endDate"].Value)
                    )
                );
            }
        }

        return releaseLifecycleInfos;
    }
}