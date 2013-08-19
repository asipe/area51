using System;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace psthreadtest {
  public class Runner {
    private bool Terminated {
      get {
        lock (mLock) {
          return mTerminated;
        }
      }
      set {
        lock (mLock) {
          mTerminated = value;
        }
      }
    }

    public void Start() {
      mThread = new Thread(DoWork);
      mThread.Start();
    }

    public void ExecuteMe(Action action) {
      action();
    }

    public void ExecuteMe<T>(Action<T> action, T args) {
      action(args);
    }

    private void DoWork() {
      while (!Terminated) {
        try {
          var runspace = RunspaceFactory.CreateRunspace();
          runspace.Open();

          var pipeline = runspace.CreatePipeline();
          pipeline.Commands.AddScript("get-process");
          pipeline.Commands.Add("Out-String");
          var results = pipeline.Invoke();

          runspace.Close();

          foreach (var result in results)
            Console.WriteLine(result.ToString());

          Console.WriteLine("Here");
        } catch (Exception e) {
          Console.WriteLine("e: {0}", e);
        }
        Thread.Sleep(1000);
      }
    }

    public void Stop() {
      Terminated = true;
      mThread.Join();
    }

    private readonly object mLock = new object();
    private bool mTerminated;
    private Thread mThread;
  }
}