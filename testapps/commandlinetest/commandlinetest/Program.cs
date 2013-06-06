using System;
using CommandLine;
using CommandLine.Text;

namespace commandlinetest {
  internal class CommitSubOptions {
    [Option('a', "all", HelpText = "Tell the command to automatically stage files all files")]
    public bool All{get;set;}

    [Option('u', "untracked", HelpText = "Tell the command to automatically stage untracked files")]
    public bool Untracked{get;set;}

    [HelpVerbOption]
    public string GetUsage(string verb) {
      Console.WriteLine("verb: {0}", verb);
      return "hi";
      //return HelpText.AutoBuild(this, verb);
    }
  }

  internal class Options {
    [VerbOption("commit", HelpText = "Record changes to the repository.")]
    public CommitSubOptions CommitVerb{get;set;} 
    
    [VerbOption("blah", HelpText = "blah blah blah")]
    public CommitSubOptions BlahVerb{get;set;}

    [HelpVerbOption]
    public string GetUsage(string verb) {
      return HelpText.AutoBuild(this, verb);
    }
  }

  internal class Program {
    private static void Main(string[] args) {
      var options = new Options();
      Parser.Default.ParseArguments(args, options,
                                    (verb, subOptions) => {
                                      if (verb == "commit") {
                                        var commitSubOptions = (CommitSubOptions)subOptions;
                                        Console.WriteLine("commitSubOptions.All: {0}", commitSubOptions.All);
                                        Console.WriteLine("commitSubOptions.Untracked: {0}", commitSubOptions.Untracked);
                                      }
                                    });
    }
  }
}