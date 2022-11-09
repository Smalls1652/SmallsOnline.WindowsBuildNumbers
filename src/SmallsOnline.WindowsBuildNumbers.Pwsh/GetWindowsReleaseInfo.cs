using System.Management.Automation;
using SmallsOnline.WindowsBuildNumbers.Lib.Helpers;
using SmallsOnline.WindowsBuildNumbers.Lib.Models;

namespace SmallsOnline.WindowsBuildNumbers.Pwsh;

[Cmdlet(verbName: VerbsCommon.Get, nounName: "WindowsReleaseInfo")]
public class GetWindowsBuildNumbers : Cmdlet
{
    [Parameter(Mandatory = false, Position = 0)]
    public MajorWindowsVersion WindowsVersion { get; set; } = MajorWindowsVersion.Windows10;

    protected override void BeginProcessing()
    {
        base.BeginProcessing();
    }

    protected override void ProcessRecord()
    {
        ReleaseInfoGetter releaseInfoGetter = new();

        WriteVerbose($"Getting release info for '{WindowsVersion}'.");
        ReleaseInfo[] releases = releaseInfoGetter.GetWindowsReleaseInfoAsync(
            windowsVersion: WindowsVersion
        ).GetAwaiter().GetResult();

        foreach (ReleaseInfo releaseItem in releases)
        {
            WriteObject(releaseItem);
        }

        releaseInfoGetter.Dispose();
    }

    protected override void StopProcessing()
    {
        base.StopProcessing();
    }
}