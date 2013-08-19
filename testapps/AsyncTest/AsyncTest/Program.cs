using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTest {
  internal class Program {
    private static void Main(string[] args) {
      Console.WriteLine("Thread.CurrentThread.ManagedThreadId(main-enter): {0}", Thread.CurrentThread.ManagedThreadId);
      var task = Execute();
      Console.WriteLine("Thread.CurrentThread.ManagedThreadId(main-post1): {0}", Thread.CurrentThread.ManagedThreadId);

      task
        .ContinueWith(t => {
          if (t.IsFaulted)
            Console.WriteLine("FAULTED!!!");
          Console.WriteLine("Thread.CurrentThread.ManagedThreadId(done): {0}", Thread.CurrentThread.ManagedThreadId);
          Console.WriteLine("t.Result: {0}", t.Result);
        })
        .Wait();
      Console.WriteLine("Thread.CurrentThread.ManagedThreadId(main-post2): {0}", Thread.CurrentThread.ManagedThreadId);
    }

    private static async Task<string> Execute() {
      var client = new WebClient();
      return await client.DownloadStringTaskAsync((string)null);//"http://www.google.comasdf");
    }

    //private static async Task Execute() {
    //  Console.WriteLine("execute enter");
    //  await Task.Run(() => {
    //    Console.WriteLine("Thread.CurrentThread.ManagedThreadId (pre): {0}", Thread.CurrentThread.ManagedThreadId);
    //    Thread.Sleep(30000);
    //    Console.WriteLine("Thread.CurrentThread.ManagedThreadId (post): {0}", Thread.CurrentThread.ManagedThreadId);
    //  });
    //  Console.WriteLine("execute exit");
    //}
  }
}