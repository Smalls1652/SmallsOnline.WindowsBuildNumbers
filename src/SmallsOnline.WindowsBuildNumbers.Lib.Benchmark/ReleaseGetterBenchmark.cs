using BenchmarkDotNet.Attributes;
using SmallsOnline.WindowsBuildNumbers.Lib.Helpers;
using SmallsOnline.WindowsBuildNumbers.Lib.Models;

namespace SmallsOnline.WindowsBuildNumbers.Lib.Benchmark;

[MemoryDiagnoser(displayGenColumns: true)]
public class ReleaseGetterBenchmark
{
    private readonly ReleaseInfoGetter _releaseInfoGetter;

    public ReleaseGetterBenchmark()
    {
        _releaseInfoGetter = new();
    }

    public ReleaseGetterBenchmark(ReleaseInfoGetter releaseInfoGetter)
    {
        _releaseInfoGetter = releaseInfoGetter;
    }

    [Benchmark]
    public async Task<ReleaseInfo[]> GetReleaseInfoWindows10Async_Benchmark() => await _releaseInfoGetter.GetWindowsReleaseInfoAsync(MajorWindowsVersion.Windows10);

    [Benchmark]
    public async Task<ReleaseInfo[]> GetReleaseInfoWindows11Async_Benchmark() => await _releaseInfoGetter.GetWindowsReleaseInfoAsync(MajorWindowsVersion.Windows11);
}