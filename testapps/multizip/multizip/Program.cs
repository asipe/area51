using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace multizip {
  internal class Program {
    private static void Main(string[] args) {
      try {
        Execute(args);
      } catch (Exception e) {
        Console.WriteLine(e);
      }
    }

    private static void Execute(string[] args) {
      _Root = args[0];
      Console.WriteLine("path: {0}", _Root);
      var runnerCount = int.Parse(args[1]);
      Console.WriteLine("runnerCount: {0}", runnerCount);
      var runnerType = args[2];
      Console.WriteLine("runnerType: {0}", runnerType);

      var sw = Stopwatch.StartNew();

      var items = Directory
        .GetDirectories(_Root)
        .Select(dir => new Item {
                                  Source = dir,
                                  Dest = Path.Combine(_Root, dir.Replace(_Root, "") + ".zip")
                                })
        .ToArray();

      switch (runnerType) {
        case "file":
          new FileZip().Exeucte(items, runnerCount);
          break;
        case "memory":
          new MemoryZip().Exeucte(items, runnerCount);
          break;
        default:
          throw new Exception("Unknown type");
      }

      Console.WriteLine("sw.ElapsedMilliseconds: {0}", sw.ElapsedMilliseconds);
    }

    private static string _Root;
  }
}