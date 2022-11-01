namespace SmallsOnline.WindowsBuildNumbers.Lib.Models;

/// <summary>
/// Interface for Windows 10/11 release builds.
/// </summary>
public interface IReleaseBuild
{
    /// <summary>
    /// The build number of the release.
    /// </summary>
    string BuildNumber { get; set; }

    /// <summary>
    /// The servicing channels that the release is available on.
    /// </summary>
    string[] ServicingChannels { get; set; }

    /// <summary>
    /// The date the build was released.
    /// </summary>
    DateTimeOffset ReleaseDate { get; set; }

    /// <summary>
    /// Whether the release is a "Patch Tuesday" release.
    /// </summary>
    bool IsPatchTuesdayRelease { get; }

    /// <summary>
    /// The KB article ID for the release.
    /// </summary>
    string? KbArticleId { get; set; }

    /// <summary>
    /// The KB article URL for the release.
    /// </summary>
    Uri? KbArticleUrl { get; set; }
}