using System;
using System.Runtime.InteropServices;
using System.Threading;
using BinObjCleaner.Commands;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace BinObjCleaner
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidPackageString)]
    public sealed class DeleteBinObjCommandPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await DeleteBinObjCommand.InitializeAsync(this);
        }
    }
}
