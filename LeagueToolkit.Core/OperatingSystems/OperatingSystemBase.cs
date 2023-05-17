using System.Diagnostics;
using System.Text.RegularExpressions;
using LeagueToolkit.Core.Models;

namespace LeagueToolkit.Core.OperatingSystems;

public abstract class OperatingSystemBase
{
    protected virtual string AuthTokenRegex => "--(riotclient|remoting)-auth-token=(.*?)( --|\n|$|\")";
    protected virtual string PortRegex  => "--(riotclient-|)app-port=(.*?)( --|\n|$|\")";
    
    protected abstract string FileName { get; }
    protected abstract string GetCommandLineArgs(int pid);

    public AuthenticationInfo ExtractAuthenticationInfo(int pid)
    {
        string commandLineArgs = GetCommandLineArgs(pid);
        string commandLineOutput = GetCommandLineOutput(FileName, commandLineArgs);
        
        return string.IsNullOrEmpty(commandLineOutput) ? null : ExtractArguments(commandLineOutput);
    }

    public AuthenticationInfo ExtractArguments(string commandLine)
    {
        MatchCollection authToken = Regex.Matches(commandLine, AuthTokenRegex);
        MatchCollection port = Regex.Matches(commandLine, PortRegex);

        AuthenticationInfo authInfo = new();
        
        foreach (Match match in authToken)
        {
            if (match.Groups.Count != 4)
                continue;

            switch (match.Groups[1].Value)
            {
                case "riotclient":
                    authInfo.ClientAuthToken = match.Groups[2].Value;
                    break;
                case "remoting":
                    authInfo.RemoteAuthToken = match.Groups[2].Value;
                    break;
            }
        }

        foreach (Match match in port)
        {
            if (match.Groups.Count != 4)
                continue;

            switch (match.Groups[1].Value)
            {
                case "riotclient-":
                    authInfo.ClientPort = Convert.ToInt32(match.Groups[2].Value);
                    break;
                case "":
                    authInfo.RemotePort = Convert.ToInt32(match.Groups[2].Value);
                    break;
            }
        }

        return authInfo;
    }

    private string GetCommandLineOutput(string fileName, string command)
    {
        string output = string.Empty;
        
        try
        {
            ProcessStartInfo startInfo = new()
            {
                Verb = "runas",
                FileName = fileName,
                Arguments = command,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = false
            };

            Process proc = Process.Start(startInfo);
            output = proc?.StandardOutput.ReadToEnd();
            
            proc?.WaitForExit(5000);

            return output;
        }
        catch (Exception)
        {
            return output;
        }
    }
}