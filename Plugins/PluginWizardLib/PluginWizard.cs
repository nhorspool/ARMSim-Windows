using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Microsoft.VisualStudio.TemplateWizard;

using EnvDTE;
//using EnvDTE80;
using VSLangProj;
//using VSLangProj80;
using Microsoft.Win32;

namespace PluginWizardLib
{
    public class PluginWizard : IWizard
    {
        // This method is called before opening any item that  has the OpenInEditor attribute.
        public void BeforeOpeningFile(ProjectItem projectItem) { }

        public void ProjectFinishedGenerating(Project project)
        {
            //MessageBox.Show("starting");
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey armSimKey = rk.OpenSubKey(@"SOFTWARE\University of Victoria\ARMSim.150");

            if (armSimKey == null) return;

            string interfaceDLL = null;
            string installDirectory = null;
            using (armSimKey)
            {
                object obj = armSimKey.GetValue(@"Install Directory");
                if (obj is string)
                {
                    installDirectory = (string)obj;
                    interfaceDLL = installDirectory + @"\ARMPluginInterfaces.dll";
                }
            }

            if (installDirectory == null)
            {
                installDirectory = @"C:\Program Files\University of Victoria\ARMSim.150";
                interfaceDLL = installDirectory + @"\ARMPluginInterfaces.dll";
            }

            if (File.Exists(interfaceDLL))
            {
                //EnvDTE.Project proj2 = (EnvDTE.Project)project.Object;
                VSLangProj.VSProject proj = (project.Object as VSLangProj.VSProject);
                proj.References.Add(interfaceDLL);
                proj.References.Add("System.Windows.Forms");
                proj.References.Add("System.Drawing");

//                config = proj.Project.ConfigurationManager.ActiveConfiguration;
                foreach (EnvDTE.Configuration config in proj.Project.ConfigurationManager)
                {
                    EnvDTE.Properties configProps = config.Properties;
                    EnvDTE.Property prop = configProps.Item("OutputPath");
                    prop.Value = installDirectory;
                }//foreach

            }
        }//ProjectFinishedGenerating

        // This method is only called for item templates, not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem projectItem) { }

        // This method is called after the project is created.
        public void RunFinished() {}

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            //try
            //{
            //}
            //catch (Exception)
            //{
            //}
        }//RunStarted

        // This method is only called for item templates, not for project templates.
        public bool ShouldAddProjectItem(string filePath) { return true; }

    }//class PluginWizard
}
