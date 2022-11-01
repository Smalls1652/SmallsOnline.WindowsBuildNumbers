using System.Text.Json.Serialization;
using SmallsOnline.WindowsBuildNumbers.Lib.Helpers;

namespace SmallsOnline.WindowsBuildNumbers.Lib.Models;

/// <summary>
/// Holds information about a Windows 10/11 release build.
/// </summary>
public class ReleaseBuild : IReleaseBuild
{
    [JsonConstructor()]
    public ReleaseBuild() {}

    public ReleaseBuild(string buildNumber, string[] servicingChannels, DateTimeOffset releaseDate, string? kbArticleId = null, Uri? kbArticleUrl = null)
    {
        BuildNumber = buildNumber;
        ServicingChannels = servicingChannels;
        ReleaseDate = releaseDate;
        KbArticleId = kbArticleId;
        KbArticleUrl = kbArticleUrl;
    }

    /// <inheritdoc />
    [JsonPropertyName("buildNumber")]
    public string BuildNumber { get; set; } = null!;

    /// <inheritdoc />
    [JsonPropertyName("servicingChannels")]
    public string[] ServicingChannels { get; set; } = null!;

    /// <inheritdoc />
    [JsonPropertyName("releaseDate")]
    public DateTimeOffset ReleaseDate { get; set; }

    /// <inheritdoc />
    public bool IsPatchTuesdayRelease
    {
        get => DateTimeHelpers.GetSecondTuesdayOfTheMonth(ReleaseDate) == ReleaseDate;
    }

    /// <inheritdoc />
    [JsonPropertyName("kbArticleId")]
    public string? KbArticleId { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("kbArticleUrl")]
    public Uri? KbArticleUrl { get; set; }

    public override string ToString()
    {
        return BuildNumber;
    }
}