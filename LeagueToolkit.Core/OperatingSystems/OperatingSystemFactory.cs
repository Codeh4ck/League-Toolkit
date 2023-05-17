using System.Runtime.InteropServices;

namespace LeagueToolkit.Core.OperatingSystems;

public static class OperatingSystemFactory
{
    public static OperatingSystemBase GetOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return new WindowsOperatingSystem();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return new LinuxOperatingSystem();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return new OsxOperatingSystem();

        return null;
    }
}