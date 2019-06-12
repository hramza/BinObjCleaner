using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace BinObjCleaner.Commands
{
    internal sealed class DeleteBinObjCommand : CommandBase
    {
        private DeleteBinObjCommand(OleMenuCommandService commandService, DTE2 dte)
        {
            _dte = dte;

            var command = new CommandID(PackageGuids.guidDeleteBinObjCommandPackage, PackageIds.CmdId);
            var menuItem = new OleMenuCommand(Execute, command);

            commandService.AddCommand(menuItem);
        }

        private void Execute(object sender, EventArgs e)
        {
            try
            {
                foreach (EnvDTE.Project project in GetAllProjects())
                {
                    string root = GetProjectRootFolder(project);

                    if (string.IsNullOrEmpty(root))
                    {
                        return;
                    }

                    Delete(Path.Combine(root, "bin"), Path.Combine(root, "obj"));
                }

                _dte.StatusBar.Text = "bin & obj folders have been successfully deleted !";
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        public static DeleteBinObjCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;

            Instance = new DeleteBinObjCommand(commandService, dte);
        }
    }
}
