using System;
using System.IO;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;


namespace Template.Wizards
{

  public class ChildWizard : IWizard
  {
    //
    private DTE2 _dte2;
    private Solution2 _solution = null;
    private Project _project = null;


    public void BeforeOpeningFile(ProjectItem projectItem)
    {
      Logger.Log("ChildWizard ~ BeforeOpeningFile, start");
    }

    public void ProjectFinishedGenerating(Project project)
    {
      Logger.Log("ChildWizard ~ ProjectFinishedGenerating, start");
      _project = project;
    }

    public void ProjectItemFinishedGenerating(ProjectItem projectItem)
    {
      Logger.Log("ChildWizard ~ ProjectItemFinishedGenerating, start");
    }

    public void RunFinished()
    {
      Logger.Log("ChildWizard ~ Run finished, start");
      // Remove the tmp root project
      if (_project.Name.Equals(RootWizard._tmp_root))
        _solution.Remove(_project);
    }

    // Retrieve global replacement parameters
    public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, 
                            WizardRunKind runKind, object[] customParams)
    {
      // The DTE and solution
      this._dte2 = automationObject as DTE2;
      _solution = (Solution2)_dte2.Solution;

      Logger.Log("ChildWizard ~ RunStarted, start");
      Logger.Log("ChildWizard ~ RunStarted, safeprojectname=" + replacementsDictionary["$safeprojectname$"]);

      // Add custom parameters from Root Wizard
      replacementsDictionary.Add("$saferootprojectname$", RootWizard.GlobalDictionary["$saferootprojectname$"]);
      Logger.Log("ChildWizard ~ RunStarted, saferootprojectname=" + replacementsDictionary["$saferootprojectname$"]);
    }

    public bool ShouldAddProjectItem(string filePath)
    {
        return true;
    }

  }
}