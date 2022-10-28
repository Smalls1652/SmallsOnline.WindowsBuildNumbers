using System.Text.Json.Serialization;

namespace SmallsOnline.WindowsBuildNumbers.Lib.Models;

public class ReleaseLifecycleInfo : IReleaseLifecycleInfo
{
    [JsonConstructor()]
    public ReleaseLifecycleInfo() {}

    public ReleaseLifecycleInfo(string releaseName, DateTimeOffset endOfLifeDate)
    {
        ReleaseName = releaseName;
        EndOfLifeDate = endOfLifeDate;
    }

    /// <inheritdoc />
    [JsonPropertyName("releaseName")]
    public string ReleaseName { get; set; } = null!;

    /// <inheritdoc />
    [JsonPropertyName("endOfLifeDate")]
    public DateTimeOffset EndOfLifeDate { get; set; }
}