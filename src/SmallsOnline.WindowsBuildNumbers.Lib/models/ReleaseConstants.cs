namespace SmallsOnline.WindowsBuildNumbers.Lib.Models;

/// <summary>
/// Constant values to use for getting release information.
/// </summary>
public static class ReleaseConstants
{
    /// <summary>
    /// The release info page URL for Windows 10.
    /// </summary>
    private static readonly Uri _windows10ReleaseInfoUrl = new("https://learn.microsoft.com/en-us/windows/release-health/release-information");

    /// <summary>
    /// The release info page URL for Windows 11.
    /// </summary>
    private static readonly Uri _windows11ReleaseInfoUrl = new("https://learn.microsoft.com/en-us/windows/release-health/windows11-release-information");

    /// <summary>
    /// The consumer lifecycle page URL for Windows 10.
    /// </summary>
    private static readonly Uri _windows10ConsumerLifeCycleUrl = new("https://learn.microsoft.com/en-us/lifecycle/products/windows-10-home-and-pro");

    /// <summary>
    /// The enterprise lifecycle page URL for Windows 10.
    /// </summary>
    private static readonly Uri _windows10EnterpriseLifeCycleUrl = new("https://learn.microsoft.com/en-us/lifecycle/products/windows-10-enterprise-and-education");

    /// <summary>
    /// The consumer lifecycle page URL for Windows 11.
    /// </summary>
    private static readonly Uri _windows11ConsumerLifeCycleUrl = new("https://learn.microsoft.com/en-us/lifecycle/products/windows-11-home-and-pro");

    /// <summary>
    /// The enterprise lifecycle page URL for Windows 11.
    /// </summary>
    private static readonly Uri _windows11EnterpriseLifeCycleUrl = new("https://learn.microsoft.com/en-us/lifecycle/products/windows-11-enterprise-and-education");

    /// <summary>
    /// The release info page URL for Windows 10.
    /// </summary>
    public static Uri Windows10ReleaseInfoUrl
    {
        get => _windows10ReleaseInfoUrl;
    }

    /// <summary>
    /// The consumer lifecycle page URL for Windows 10.
    /// </summary>
    public static Uri Windows10ConsumerLifeCycleUrl
    {
        get => _windows10ConsumerLifeCycleUrl;
    }

    /// <summary>
    /// The enterprise lifecycle page URL for Windows 10.
    /// </summary>
    public static Uri Windows10EnterpriseLifeCycleUrl
    {
        get => _windows10EnterpriseLifeCycleUrl;
    }

    /// <summary>
    /// The release info page URL for Windows 11.
    /// </summary>
    public static Uri Windows11ReleaseInfoUrl
    {
        get => _windows11ReleaseInfoUrl;
    }

    /// <summary>
    /// The consumer lifecycle page URL for Windows 11.
    /// </summary>
    public static Uri Windows11ConsumerLifeCycleUrl
    {
        get => _windows11ConsumerLifeCycleUrl;
    }

    /// <summary>
    /// The enterprise lifecycle page URL for Windows 11.
    /// </summary>
    public static Uri Windows11EnterpriseLifeCycleUrl
    {
        get => _windows11EnterpriseLifeCycleUrl;
    }
}