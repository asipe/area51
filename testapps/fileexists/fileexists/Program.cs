using System;
using System.IO;

namespace fileexists {
  internal class Program {
    private static void Main(string[] args) {
      try {
        Console.WriteLine("args[0]: {0}", args[0]);
        Console.WriteLine("File.Exists(args[0]): {0}", File.Exists(args[0]));
      } catch (Exception e) {
        do {
          Console.WriteLine("e.Message: {0}", e.Message);
          Console.WriteLine("e.StackTrace: {0}", e.StackTrace);
          e = e.InnerException;
        } while (e != null);
      }
    }
  }
}