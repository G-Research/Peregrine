namespace Peregrine.ValueTypes.Test

open BenchmarkDotNet.Configs
open BenchmarkDotNet.Jobs
open BenchmarkDotNet.Running
open FsUnit

[<RequireQualifiedAccess>]
module BenchmarkDotNetHelpers =

    /// A BenchmarkDotNet config that does no warm up and only runs once.
    /// This is intended for running tests that check that code doesn't allocate; it shouldn't be used for actual benchmarking.
    let coldStartSingleRunConfig =
        ManualConfig.Create(DefaultConfig.Instance)
            //.With(ConfigOptions.DisableOptimizationsValidator) // We don't mind testing for allocation free code in DEBUG.
            .With(Job.Default
                      .WithLaunchCount(1)
                      .WithIterationCount(1))

    /// Run the benchmarks from the given class and check that none of them allocate any heap memory
    let assertAllocationFree<'benchmark> () =
        BenchmarkRunner.Run<'benchmark>(coldStartSingleRunConfig).Reports
        |> Seq.map (fun report -> report.GcStats.BytesAllocatedPerOperation)
        |> Seq.iter (should equal 0)
