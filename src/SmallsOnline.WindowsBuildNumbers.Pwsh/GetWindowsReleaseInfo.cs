using System.Management.Automation;
using SmallsOnline.WindowsBuildNumbers.Lib.Helpers;
using SmallsOnline.WindowsBuildNumbers.Lib.Models;

namespace SmallsOnline.WindowsBuildNumbers.Pwsh;

/// <summary>
/// Get release information for Windows 10/11 feature updates.
/// </summary>
[Cmdlet(verbName: VerbsCommon.Get, nounName: "WindowsReleaseInfo")]
public class GetWindowsBuildNumbers : Cmdlet
{
    /// <summary>
    /// The major version of Windows to get release information for.
    /// </summary>
    [Parameter(Mandatory = false, Position = 0)]
    public MajorWindowsVersion WindowsVersion { get; set; } = MajorWindowsVersion.Windows10;

    private readonly ReleaseInfoGetter _releaseInfoGetter = new();

    protected override void ProcessRecord()
    {
        WriteVerbose($"Getting release info for '{WindowsVersion}'.");
        ReleaseInfo[] releases = _releaseInfoGetter.GetWindowsReleaseInfoAsync(
            windowsVersion: WindowsVersion
        ).GetAwaiter().GetResult();

        foreach (ReleaseInfo releaseItem in releases)
        {
            WriteObject(releaseItem);
        }

        _releaseInfoGetter.Dispose();
    }
}