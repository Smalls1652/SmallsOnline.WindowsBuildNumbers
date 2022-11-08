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
        options: RegexOptions.Compiled
    );

    private static readonly Regex _releaseLifecycleTableDataRegex = new(
        pattern:
        @"<tr>\n\s+<td>Version (?'versionNumber'.{4})<\/td>\n\s+<td .+?>\n\s+.*(?'startDate'\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}-\d{2}:\d{2}).+\n\s+<\/td>\n\s+<td .+?>\n\s+.*(?'endDate'\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}-\d{2}:\d{2}).*\n\s+<\/td>\n\s+<\/tr>",
        options: RegexOptions.Compiled
    );

    /// <summary>
    /// Parse the release lifecycle information page's contents for Windows 10/11.
    /// </summary>
    /// <param name="releaseLifecyclePageContent">The content of the lifecycle page.</param>
    /// <returns>A collection of regex matches.</returns>
    /// <exception cref="Exception"></exception>
    public static Match ParseReleaseLifecycleContent(string releaseLifecyclePageContent)
    {
        // Get all the matches for each release lifecycle table.
        Match releaseLifecycleMatch = _releaseLifecycleTableRegex.Match(releaseLifecyclePageContent);

        // If there are no matches, throw an exception.
        if (!releaseLifecycleMatch.Success)
        {
            throw new Exception("No release lifecycle data found in the release lifecycle page content.");
        }

        return releaseLifecycleMatch;
    }

    /// <summary>
    /// Parse the release lifecycle table data for Windows 10/11.
    /// </summary>
    /// <param name="releaseLifecycleMatch">The collection of matches to parse.</param>
    /// <returns>A list of <see cref="ReleaseLifecycleInfo" /> items.</returns>
    public static ReleaseLifecycleInfo[] ParseReleaseLifecycleTableData(Match releaseLifecycleMatch)
    {
        // Get the table data.
        string tableData = releaseLifecycleMatch.Groups["tableData"].Value;

        // Get all the matches for each release lifecycle table data.
        MatchCollection releaseLifecycleTableDataMatches = _releaseLifecycleTableDataRegex.Matches(tableData);

        // Initialize the list to hold ReleaseLifecycleInfo items.
        ReleaseLifecycleInfo[] releaseLifecycleInfos = new ReleaseLifecycleInfo[releaseLifecycleTableDataMatches.Count];

        for (int i = 0; i < releaseLifecycleTableDataMatches.Count; i++)
        {
            releaseLifecycleInfos[i] = new(
                releaseName: releaseLifecycleTableDataMatches[i].Groups["versionNumber"].Value,
                endOfLifeDate: DateTimeOffset.Parse(releaseLifecycleTableDataMatches[i].Groups["endDate"].Value)
            );
        }

        /*
        // Loop through each match.
        foreach (Match tableDataMatch in releaseLifecycleMatch.AsEnumerable())
        {
            // Get the table data.
            string tableData = tableDataMatch.Groups["tableData"].Value;

            // Get all the matches for each release lifecycle table data.
            MatchCollection releaseLifecycleTableDataMatches = _releaseLifecycleTableDataRegex.Matches(tableData);

            // Loop through each match from the table data.
            foreach (Match releaseLifecycleTableDataMatch in releaseLifecycleTableDataMatches.AsEnumerable())
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
        */

        return releaseLifecycleInfos;
    }
}