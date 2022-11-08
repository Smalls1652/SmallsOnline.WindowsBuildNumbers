namespace SmallsOnline.WindowsBuildNumbers.Lib.Models;

/// <summary>
/// Interface for Windows 10/11 release information.
/// </summary>
public interface IReleaseInfo
{
    /// <summary>
    /// The name of the release.
    /// </summary>
    string ReleaseName { get; set; }

    /// <summary>
    /// The end-of-life date for the consumer release.
    /// </summary>
    DateTimeOffset? ConsumerEoLDate { get; set; }

    /// <summary>
    /// Whether the consumer release is end-of-life.
    /// </summary>
    bool ConsumerIsEoL { get; }

    /// <summary>
    /// The end-of-life date for the enterprise release.
    /// </summary>
    DateTimeOffset? EnterpriseEoLDate { get; set; }

    /// <summary>
    /// Whether the enterprise release is end-of-life.
    /// </summary>
    bool EnterpriseIsEoL { get; }

    /// <summary>
    /// Builds for the release.
    /// </summary>
    IEnumerable<ReleaseBuild>? ReleaseBuilds { get; set; }
}