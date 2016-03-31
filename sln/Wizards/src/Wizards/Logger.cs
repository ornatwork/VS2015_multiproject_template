using System;
using System.IO;
using System.Diagnostics;


namespace Template.Wizards
{
  public class Logger
  {
    // default log file name 
    static string _logFileName = RootWizard._solutionFolder + "\\template.log";

    /// <summary>
    /// Set the log file name including path
    /// </summary>
    public static void FileName(string FileName)
    {
      _logFileName = FileName;
    }

    /// <summary>
    /// Logs out the passed text to the logfile
    /// </summary>
    public static void Log( string Text )
    {
      DateTime time = DateTime.Now;
      string theTime = String.Format("{0:d/M/yyyy HH:mm:ss:fff}", time);
      string textlineOut = "~~~ " + theTime + ", " + Text + Environment.NewLine;

      // Log it
      Console.WriteLine(textlineOut);
      Trace.WriteLine(textlineOut);
      // To file 
      File.AppendAllText(_logFileName, textlineOut);
    }

  }
}
