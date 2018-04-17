using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    extern alias NewVersion;
    extern alias OldVersion;

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Test>();
            Console.ReadLine();
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(MarkdownExporter.GitHub); // ベンチマーク結果を書く時に出力させとくとベンリ
            Add(MemoryDiagnoser.Default);

            // ShortRunを使うとサクッと終わらせられる、デフォルトだと本気で長いので短めにしとく。
            // ShortRunは LaunchCount=1  TargetCount=3 WarmupCount = 3 のショートカット
            // Add(Job.ShortRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Test
    {
        private readonly Person _object;
        private readonly string _callback;

        public Test()
        {
            _object = new Person()
            {
                Age = 99,
                Name = "hanako"
            };

            _callback = "callback";
        }

        [Benchmark]
        public byte[] OldVersion()
        {
            return OldVersion::Utf8Json.Jsonp.Serialize(_callback, _object);
        }

        [Benchmark]
        public byte[] NewVersion()
        {
            return NewVersion::Utf8Json.Jsonp.Serialize(_callback, _object);
        }
    }

    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }
}
