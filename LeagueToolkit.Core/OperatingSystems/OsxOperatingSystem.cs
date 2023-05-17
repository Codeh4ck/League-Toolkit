namespace LeagueToolkit.Core.OperatingSystems;

public class OsxOperatingSystem : OperatingSystemBase
{
    protected override string FileName => "/bin/bash";
    protected override string GetCommandLineArgs(int pid) => $"/C \"ps -ww -fp {pid}\"";
}