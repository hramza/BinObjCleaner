namespace BinObjCleaner
{
    using System;

    /// <summary>
    /// Helper class that exposes all GUIDs used across VS Package.
    /// </summary>
    internal sealed partial class PackageGuids
    {
        public const string guidPackageString = "47db9fed-ae8a-4e29-9950-342d0ac12b2b";
        public const string guidDeleteBinObjCommandPackageCmdSet = "6c62c01c-a902-4852-ae94-727a5a0b67e4";
        public static Guid guidPackage = new Guid(guidPackageString);
        public static Guid guidDeleteBinObjCommandPackage = new Guid(guidDeleteBinObjCommandPackageCmdSet);
    }
    /// <summary>
    /// Helper class that encapsulates all CommandIDs uses across VS Package.
    /// </summary>
    internal sealed partial class PackageIds
    {
        public const int CmdId = 0x0064;
    }
}
