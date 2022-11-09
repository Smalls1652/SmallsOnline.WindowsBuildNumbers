using System.Management.Automation;
using SmallsOnline.WindowsBuildNumbers.Lib.Helpers;
using SmallsOnline.WindowsBuildNumbers.Lib.Models;

namespace SmallsOnline.WindowsBuildNumbers.Pwsh;

/// <summary>
/// Get release information for Windows 10/11 feature updates.
/// </summary>
[Cmdlet(verbName: VerbsCommon.Get, nounName: "WindowsReleaseInfo")]
[OutputType(typeof(ReleaseInfo[]))]
public class GetWindowsBuildNumbers : Cmdlet
{
    /// <summary>
    /// The major version of Windows to get release information for.
    /// </summary>
    [Parameter(Mandatory = false, Position = 0)]
    public MajorWindowsVersion WindowsVersion { get; set; } = MajorWindowsVersion.Windows10;

    /// <summary>
    /// The name of the feature update release to get information for.
    /// </summary>
    [Parameter(Mandatory = false, Position = 1)]
    public string ReleaseName { get; set; } = default!;

    private readonly ReleaseInfoGetter _releaseInfoGetter = new();

    protected override void ProcessRecord()
    {
        // Get the release information for the specified version of Windows.
        // Todo: Look into optimizing the async call.
        WriteVerbose($"Getting release info for '{WindowsVersion}'.");
        ReleaseInfo[] releases = _releaseInfoGetter.GetWindowsReleaseInfoAsync(
            windowsVersion: WindowsVersion
        ).GetAwaiter().GetResult();

        if (string.IsNullOrEmpty(ReleaseName))
        {
            // If no release name was specified, return all releases.

            // Write each release to the standard output.
            foreach (ReleaseInfo releaseItem in releases)
            {
                WriteObject(releaseItem);
            }
        }
        else
        {
            // If a release name was specified, return only the release with that name.
            try
            {
                ReleaseInfo foundReleaseItem = GetSpecificReleaseInfo(
                    releaseName: ReleaseName,
                    releases: releases
                );

                WriteObject(foundReleaseItem);
            }
            catch (NullReferenceException)
            {
                // If the release name was not found, throw a terminating error.
                ThrowTerminatingError(
                    new(
                        exception: new($"Release '{ReleaseName}' not found."),
                        errorId: "ReleaseNotFound",
                        errorCategory: ErrorCategory.ObjectNotFound,
                        targetObject: ReleaseName
                    )
                );
            }
        }

        // Dispose the ReleaseInfoGetter.
        _releaseInfoGetter.Dispose();
    }

    /// <summary>
    /// Attempt to get the release information for a specified release name.
    /// </summary>
    /// <param name="releaseName">The name of the feature update release.</param>
    /// <param name="releases">Releases that have been parsed already.</param>
    /// <returns>A single <see cref="ReleaseInfo" /> item for the specified release.</returns>
    /// <exception cref="NullReferenceException">The specified release could not be found.</exception>
    private ReleaseInfo GetSpecificReleaseInfo(string releaseName, ReleaseInfo[] releases)
    {
        // Convert the input releases array to a list.
        List<ReleaseInfo> releaseList = releases.ToList();

        // Find the release with the specified name in the list.
        ReleaseInfo? releaseInfo = releaseList.Find(release => release.ReleaseName == releaseName);

        // If the release was not found, throw an exception.
        if (releaseInfo is null)
        {
            throw new NullReferenceException(
                message: $"Could not find release '{releaseName}'."
            );
        }

        return releaseInfo;
    }
}