namespace SmallsOnline.WindowsBuildNumbers.Lib.Models;

/// <summary>
/// Interface for information about a Windows 10/11 release lifecycle.
/// </summary>
public interface IReleaseLifecycleInfo
{
    /// <summary>
    /// The name of the release.
    /// </summary>
    string ReleaseName { get; set; }

    /// <summary>
    /// The end-of-life date for the version.
    /// </summary>
    DateTimeOffset EndOfLifeDate { get; set; }
}