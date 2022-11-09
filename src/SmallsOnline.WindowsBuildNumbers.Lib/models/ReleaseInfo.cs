using System.Text.Json.Serialization;

namespace SmallsOnline.WindowsBuildNumbers.Lib.Models;

/// <summary>
/// Holds information about a Windows 10/11 release.
/// </summary>
public class ReleaseInfo : IReleaseInfo
{
    [JsonConstructor]
    public ReleaseInfo() {}

    public ReleaseInfo(string releaseName, DateTimeOffset? consumerEoLDate, DateTimeOffset? enterpriseEoLDate)
    {
        ReleaseName = releaseName;

        if (consumerEoLDate is not null)
        {
            ConsumerEoLDate = consumerEoLDate;
        }

        if (enterpriseEoLDate is not null)
        {
            EnterpriseEoLDate = enterpriseEoLDate;
        }
    }

    /// <inheritdoc />
    [JsonPropertyName("releaseName")]
    public string ReleaseName { get; set; } = null!;

    /// <inheritdoc />
    [JsonPropertyName("consumerEoLDate")]
    public DateTimeOffset? ConsumerEoLDate { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("cosumerIsEoL")]
    public bool ConsumerIsEoL
    {
        get => DateTimeOffset.Now > ConsumerEoLDate;
    }

    /// <inheritdoc />
    [JsonPropertyName("enterpriseEoLDate")]
    public DateTimeOffset? EnterpriseEoLDate { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("enterpriseIsEoL")]
    public bool EnterpriseIsEoL
    {
        get => DateTimeOffset.Now > EnterpriseEoLDate;
    }

    /// <inheritdoc />
    [JsonPropertyName("releaseBuilds")]
    public IEnumerable<ReleaseBuild>? ReleaseBuilds { get; set; }

    public override string ToString()
    {
        return ReleaseName;
    }
}