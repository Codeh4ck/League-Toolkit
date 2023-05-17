namespace LeagueToolkit.Core.OperatingSystems;

public class WindowsOperatingSystem : OperatingSystemBase
{
    protected override string FileName => "cmd.exe";
    protected override string GetCommandLineArgs(int pid) =>  $"/C \"WMIC PROCESS WHERE ProcessId={pid} GET commandline\"";
}