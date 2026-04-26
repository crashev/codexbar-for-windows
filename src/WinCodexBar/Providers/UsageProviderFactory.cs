namespace WinCodexBar.Providers;

public static class UsageProviderFactory
{
    public static IUsageProvider CreateDefault()
    {
        return IsOnPath("codexbar")
            ? new CodexBarCliUsageProvider()
            : new MockUsageProvider();
    }

    private static bool IsOnPath(string executable)
    {
        var paths = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

        var exts = OperatingSystem.IsWindows()
            ? (Environment.GetEnvironmentVariable("PATHEXT") ?? ".EXE;.CMD;.BAT")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
            : [string.Empty];

        foreach (var path in paths)
        {
            foreach (var ext in exts)
            {
                var candidate = Path.Combine(path, executable + ext.ToLowerInvariant());
                if (File.Exists(candidate))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
