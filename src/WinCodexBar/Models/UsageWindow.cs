namespace WinCodexBar.Models;

public sealed record UsageWindow(double UsedPercent, int WindowMinutes, DateTimeOffset? ResetsAt)
{
    public double RemainingPercent => Math.Max(0, 100 - UsedPercent);
}
