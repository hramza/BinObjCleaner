using System;
using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace BinObjCleaner.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
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

                    if (root == null)
                        return;

                    string bin = Path.Combine(root, "bin");
                    string obj = Path.Combine(root, "obj");

                    Delete(bin, obj);
                }

                _dte.StatusBar.Text = "bin & obj folders are successfully deleted !";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public static DeleteBinObjCommand Instance
        {
            get; private set;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;

            Instance = new DeleteBinObjCommand(commandService, dte);
        }
    }
}
