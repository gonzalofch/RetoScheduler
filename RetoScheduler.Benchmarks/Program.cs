using BenchmarkDotNet.Running;
using RetoScheduler.Benchmarks;

var summary = BenchmarkRunner.Run<GetListDayOfWeekInStringBenchmark>();
