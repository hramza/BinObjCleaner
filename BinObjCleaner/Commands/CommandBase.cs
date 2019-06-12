﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace BinObjCleaner.Commands
{
    public class CommandBase
    {
        protected DTE2 _dte;

        protected void Delete(params string[] folders)
        {
            IEnumerable<string> existingFolders = folders.Where(f => Directory.Exists(f));

            foreach (string folder in existingFolders)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories);

                if (!files.Any(f => f.EndsWith(".refresh") || _dte.SourceControl.IsItemUnderSCC(f)))
                {
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write(ex);
                    }
                }
            }
        }

        protected IEnumerable<Project> GetAllProjects()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return _dte.Solution.Projects
                  .Cast<Project>()
                  .SelectMany(GetChildProjects)
                  .Union(_dte.Solution.Projects.Cast<Project>())
                  .Where(p => { try { return !string.IsNullOrEmpty(p.FullName); } catch { return false; } });
        }

        private static IEnumerable<Project> GetChildProjects(Project parent)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                string vsProjectKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
                if (parent.Kind != vsProjectKindSolutionFolder && parent.Collection == null)
                    return Enumerable.Empty<Project>();

                if (!string.IsNullOrEmpty(parent.FullName))
                    return new[] { parent };
            }
            catch (COMException)
            {
                return Enumerable.Empty<Project>();
            }

            return parent.ProjectItems
                    .Cast<ProjectItem>()
                    .Where(p => { ThreadHelper.ThrowIfNotOnUIThread(); return p.SubProject != null; })
                    .SelectMany(p => { ThreadHelper.ThrowIfNotOnUIThread(); return GetChildProjects(p.SubProject); });
        }

        public static string GetProjectRootFolder(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrEmpty(project.FullName))
            {
                return null;
            }

            string fullPath;

            try
            {
                fullPath = project.Properties.Item("FullPath").Value as string;
            }
            catch (ArgumentException)
            {
                try
                {
                    // MFC projects
                    fullPath = project.Properties.Item("ProjectDirectory").Value as string;
                }
                catch (ArgumentException)
                {
                    fullPath = project.Properties.Item("ProjectPath").Value as string;
                }
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                return File.Exists(project.FullName) ? Path.GetDirectoryName(project.FullName) : null;
            }

            if (Directory.Exists(fullPath))
            {
                return fullPath;
            }

            if (File.Exists(fullPath))
            {
                return Path.GetDirectoryName(fullPath);
            }

            return null;
        }
    }
}
