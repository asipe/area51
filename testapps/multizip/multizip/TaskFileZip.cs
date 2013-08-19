using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace multizip {
  public class TaskFileZip {
    public void Execute(Item[] items, int runnerCount) {
      AsyncExecute(items, runnerCount).Wait();
    }

    public async Task AsyncExecute(Item[] items, int runnerCount) {
      var queue = new Queue<Item>(items);
      var tasks = new List<Task<Item>>();

      while (tasks.Any() || queue.Any()) {
        while (tasks.Count() != runnerCount && queue.Any())
          tasks.Add(CompressUsingZipFile(queue.Dequeue()));
        var task = await Task.WhenAny(tasks);
        tasks.Remove(task);
        await task;
      }
    }

    private static Task<Item> CompressUsingZipFile(Item item) {
      return Task.Factory.StartNew(() => {
        ZipFile.CreateFromDirectory(item.Source, item.Dest, CompressionLevel.Fastest, false);
        return item;
      });
    }
  }
}