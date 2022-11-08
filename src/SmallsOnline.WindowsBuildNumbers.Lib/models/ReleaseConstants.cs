namespace SmallsOnline.WindowsBuildNumbers.Lib.Models;

/// <summary>
/// Constant values to use for getting release information.
/// </summary>
public static class ReleaseConstants
{
    /// <summary>
    /// The release info page URL for Windows 10.
    /// </summary>
    public static Uri Windows10ReleaseInfoUrl { get; } = new("https://learn.microsoft.com/en-us/windows/release-health/release-information");

    /// <summary>
    /// The consumer lifecycle page URL for Windows 10.
    /// </summary>
    public static Uri Windows10ConsumerLifeCycleUrl { get; } = new("https://learn.microsoft.com/en-us/lifecycle/products/windows-10-home-and-pro");

    /// <summary>
    /// The enterprise lifecycle page URL for Windows 10.
    /// </summary>
    public static Uri Windows10EnterpriseLifeCycleUrl { get; } = new("https://learn.microsoft.com/en-us/lifecycle/products/windows-10-enterprise-and-education");

    /// <summary>
    /// The release info page URL for Windows 11.
    /// </summary>
    public static Uri Windows11ReleaseInfoUrl { get; } = new("https://learn.microsoft.com/en-us/windows/release-health/windows11-release-information");

    /// <summary>
    /// The consumer lifecycle page URL for Windows 11.
    /// </summary>
    public static Uri Windows11ConsumerLifeCycleUrl { get; } = new("https://learn.microsoft.com/en-us/lifecycle/products/windows-11-home-and-pro");

    /// <summary>
    /// The enterprise lifecycle page URL for Windows 11.
    /// </summary>
    public static Uri Windows11EnterpriseLifeCycleUrl { get; } = new("https://learn.microsoft.com/en-us/lifecycle/products/windows-11-enterprise-and-education");
}