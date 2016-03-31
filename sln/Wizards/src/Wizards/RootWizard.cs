using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90a;
using Microsoft.VisualStudio.TemplateWizard;



namespace Template.Wizards
{

  public class RootWizard : IWizard
  {
    public static Dictionary<string, string> GlobalDictionary = new Dictionary<string, string>();
    //internal readonly static Dictionary<string, string> GlobalParameters = new Dictionary<string, string>();

    private EnvDTE._DTE _dte;
    private Solution2 _solution = null;

    private string _originalDestinationFolder;
    public static string _solutionFolder;
    private string _realTemplatePath;
    public static string _tmp_root = "tmp_root";
    private static string _rootSolutionItems = "Solution Items";
    private string _desiredNamespace;
    private static bool _beenHereBefore = false;



    public void BeforeOpeningFile(ProjectItem projectItem)
    {
      Logger.Log("RootWizard - BeforeOpeningFile, Start");
    }

    public void ProjectFinishedGenerating(Project project)
    {
      Logger.Log("RootWizard - ProjectFinishedGenerating, Start");
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
      Logger.Log("RootWizard - ProjectItemFinishedGenerating, Start");
    }

    public void RunFinished()
    {
      Logger.Log("RootWizard - Runfinished, Start");

      // Make sure to only run this ones using 
      // the real template in the /real_template directory
      if (!_beenHereBefore)
      {
        _beenHereBefore = true;

        // Run the real template
        _dte.Solution.AddFromTemplate(
          _realTemplatePath,
          _solutionFolder,
          _desiredNamespace,
          false);

        //This is the old undesired folder, with nothing in it, otherwise the delete will not work !
        ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DeleteDummyDir), _originalDestinationFolder);
      }
      else
      {
        // This is the second time it finishes ( for the real solution )
        Logger.Log("RootWizard - Runfinished, adjust solution files, copy from dir=" + _solutionFolder + "\\" + _tmp_root + ", to dir=" + _solutionFolder);

        // The root //Solution Items from 
        createSolutionDirAndFiles(_rootSolutionItems, _solutionFolder + _tmp_root, _solutionFolder);

        // Then other dirs that have anything in them, skipping empty dirs
        foreach (string originPath in Directory.GetDirectories(_solutionFolder + _tmp_root, "*.*", SearchOption.AllDirectories))
        {
          Logger.Log("RootWizard - Runfinished, dir=" + originPath);
          var dirInfo = new DirectoryInfo(originPath);
          string dirName = originPath.Replace(_solutionFolder + _tmp_root + "\\", "");
          if (dirInfo.GetFiles().Count() > 0)
          {
            Directory.CreateDirectory(_solutionFolder + dirName);
            createSolutionDirAndFiles(dirName, originPath, _solutionFolder + dirName);
          }
        } // foreach

      }  // if
    }



    private void DeleteDummyDir(object oDir)
    {
      //Let the solution and dummy generated and exit...
      System.Threading.Thread.Sleep(2000);

      //Delete the original destination folder
      string dir = (string)oDir;
      Logger.Log("RootWizard - Deleting directory, dir=" + dir);
      if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
      {
        Directory.Delete(dir);
        //Util.RecursiveDelete(new DirectoryInfo(dir));
      }
    }


    public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
    {

      // Get solution dir and start the logger 
      _solutionFolder = replacementsDictionary["$solutiondirectory$"];
      Logger.Log("RootWizard - RunStarted, start");
      Logger.Log(".. namespace=" + replacementsDictionary["$registeredorganization$"] + ", safe=" + replacementsDictionary["$safeprojectname$"] );

      // VS objects
      _dte = automationObject as EnvDTE._DTE;
      _solution = (Solution2)_dte.Solution;

      // Create the desired path and namespace to generate the project at 
      string templateFilePath = (string)customParams[0];
      string vsixFilePath = Path.GetDirectoryName(templateFilePath);
      _originalDestinationFolder = replacementsDictionary["$destinationdirectory$"];
      _realTemplatePath = Path.Combine(vsixFilePath, @"real_template\sample.vstemplate"); ;
      _desiredNamespace = replacementsDictionary["$safeprojectname$"];

      // Copy from $safeprojectname$ passed in my root vstemplate
      if (GlobalDictionary.Count == 0)
      {
        GlobalDictionary["$saferootprojectname$"] = replacementsDictionary["$safeprojectname$"];
        GlobalDictionary["$template_location"] = (string)customParams[0];
      }
    }

    public bool ShouldAddProjectItem(string filePath)
    {
      return true;
    }

    /// <summary>
    /// Creates solution directory and includes files from destination
    /// </summary>
    private void createSolutionDirAndFiles(string SolutionDir, string SourcePath, string DestinationPath)
    {
      // Add a solution Folder for the //Solution Items ( from root )
      string[] dirs = SolutionDir.Split('\\');
      EnvDTE.Project solutionsFolderProject = _solution.AddSolutionFolder(dirs[0]);
      // Add sub directories if needed
      for (int i = 1; i < dirs.Count(); i++)
      {
        SolutionFolder SF = (SolutionFolder)solutionsFolderProject.Object;
        solutionsFolderProject = SF.AddSolutionFolder(dirs[i]);
      }


      // Copy and add the files 
      foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.TopDirectoryOnly))
      {
        // The filename to copy over
        string fileName = newPath.Replace(SourcePath, DestinationPath);
        // Don't add the fake project 
        if (!fileName.EndsWith("proj"))
        {
          // Strip out extra directory character
          fileName = fileName.Replace("\\\\", "\\");
          // Copy to new home on hard drive
          File.Copy(newPath, fileName, true);
          Logger.Log("Rootwizard ~~~~ Adding File to solution=" + fileName);
          // Add the item from the local hard drive to the project
          EnvDTE.ProjectItem addedFile = solutionsFolderProject.ProjectItems.AddFromFile(fileName);
          // Optionally close the pointless window to clean up the solution from the start
          addedFile.DTE.ActiveWindow.Close(vsSaveChanges.vsSaveChangesNo);
        }
      }
    }

  }  // EOC
}