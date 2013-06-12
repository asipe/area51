using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace multizip {
  public class FileZip {
    public void Exeucte(Item[] items, int runnerCount) {
      mItems = new Queue<Item>(items);
      var threads = Enumerable
        .Range(0, runnerCount)
        .Select(x => new Thread(CompressUsingZipFile))
        .ToArray();

      Array.ForEach(threads, t => t.Start());
      Array.ForEach(threads, t => t.Join());
    }

    private void CompressUsingZipFile() {
      var item = GetNextItem();
      while (item != null) {
        if (File.Exists(item.Dest))
          File.Delete(item.Dest);
        ZipFile.CreateFromDirectory(item.Source, item.Dest, CompressionLevel.Fastest, false);
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