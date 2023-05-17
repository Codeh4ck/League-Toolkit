namespace LeagueToolkit.Core.OperatingSystems;

public class LinuxOperatingSystem : OperatingSystemBase
{
    protected override string FileName => "/bin/bash";
    protected override string GetCommandLineArgs(int pid) => $"-c \"ps -ww -fp {pid}\"";
}