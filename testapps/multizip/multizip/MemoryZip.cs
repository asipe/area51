using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Snarfz.Core;

namespace multizip {
  public class MemoryZip {
    public void Exeucte(Item[] items, int runnerCount) {
      mItems = new Queue<Item>(items);
      var threads = Enumerable
        .Range(0, runnerCount)
        .Select(x => new Thread(CompressUsingMemory))
        .ToArray();

      Array.ForEach(threads, t => t.Start());
      Array.ForEach(threads, t => t.Join());
    }

    private void CompressUsingMemory() {
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

    private Item GetNextItem() {
      lock (mLock) {
        return mItems.Any() ? mItems.Dequeue() : null;
      }
    }

    private readonly object mLock = new object();
    private Queue<Item> mItems;
  }
}