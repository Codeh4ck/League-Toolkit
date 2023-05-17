using Spectre.Console;

namespace LeagueToolkit.SoloQueueReveal.Utilities;

public static class ConsoleUtility
{
    public static void WriteColorLine(Color color, string text, bool useTimestamp = false) =>
        AnsiConsole.MarkupLine(!useTimestamp
            ? $"[{color.ToMarkup()}]{text}[/]"
            : $"[{color.ToMarkup()}]({DateTime.Now:T}) - {text}[/]");

    public static void WriteColor(Color color, string text, bool useTimestamp = false) =>
        AnsiConsole.Markup(!useTimestamp
            ? $"[{color.ToMarkup()}]{text}[/]"
            : $"[{color.ToMarkup()}]({DateTime.Now:T}) - {text}[/]");
}