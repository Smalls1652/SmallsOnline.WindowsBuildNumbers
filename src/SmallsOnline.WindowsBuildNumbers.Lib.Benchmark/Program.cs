using BenchmarkDotNet.Running;

namespace SmallsOnline.WindowsBuildNumbers.Lib.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}
