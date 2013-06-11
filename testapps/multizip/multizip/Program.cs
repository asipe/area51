using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Snarfz.Core;

namespace multizip {
  public class Item {
    public string Source{get;set;}
    public string Dest{get;set;}
  }

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

      var sw = Stopwatch.StartNew();

      foreach (var dir in Directory.GetDirectories(_Root)) {
        //var cfg = new Config(dir);
        //cfg.OnDirectory += (o, a) => Console.WriteLine("a.Path: {0}", a.Path);
        //cfg.OnFile += (o, a) => Console.WriteLine("a.Path: {0}", a.Path);
        //Snarfzer.NewScanner().Start(cfg);

        var dest = Path.Combine(_Root, dir.Replace(_Root, "") + ".zip");
        _Items.Enqueue(new Item {Source = dir, Dest = dest});

        //Console.WriteLine("dest: {0}", dest);

        //ZipFile.CreateFromDirectory(dir, dest);
      }

      var threads = Enumerable
        .Range(0, runnerCount)
        .Select(x => new Thread(CompressUsingMemory))
        //.Select(x => new Thread(CompressUsingZipFile))
        .ToArray();

      Array.ForEach(threads, t => t.Start());
      Array.ForEach(threads, t => t.Join());

      Console.WriteLine("sw.ElapsedMilliseconds: {0}", sw.ElapsedMilliseconds);
    }

    private static void CompressUsingMemory() {
      var item = GetNextItem();
      while (item != null) {
        if (File.Exists(item.Dest))
          File.Delete(item.Dest);

        using (var strm = new MemoryStream()) {
          using (var archive = new ZipArchive(strm, ZipArchiveMode.Create, true)) {
            var cfg = new Config(item.Source) {ScanType = ScanType.FilesOnly};
            cfg.OnFile += (o, a) => {
              var entryPath = a.Path.Replace(item.Source + "\\", "");
              var entry = archive.CreateEntry(entryPath, CompressionLevel.Fastest);
              using (var entryStream = entry.Open()) {
                using (var fileStream = File.OpenRead(a.Path)) {
                  fileStream.CopyTo(entryStream);
                  fileStream.Close();
                }
                entryStream.Flush();
                entryStream.Close();
              }
            };
            Snarfzer.NewScanner().Start(cfg);
          }

          strm.Position = 0;
          using (var outstream = File.OpenWrite(item.Dest))
            strm.CopyTo(outstream);
        }
        item = GetNextItem();
      }
    }

    private static void CompressUsingZipFile() {
      var item = GetNextItem();
      while (item != null) {
        if (File.Exists(item.Dest))
          File.Delete(item.Dest);
        ZipFile.CreateFromDirectory(item.Source, item.Dest, CompressionLevel.Fastest, false);
        item = GetNextItem();
      }
    }

    private static Item GetNextItem() {
      lock (_Lock) {
        return _Items.Any() ? _Items.Dequeue() : null;
      }
    }

    private static readonly object _Lock = new object();
    private static readonly Queue<Item> _Items = new Queue<Item>();
    private static string _Root;
  }
}