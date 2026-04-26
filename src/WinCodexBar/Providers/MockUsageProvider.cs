using WinCodexBar.Models;

namespace WinCodexBar.Providers;

public sealed class MockUsageProvider : IUsageProvider
{
    public Task<ProviderSnapshot> GetSnapshotAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.Now;
        return Task.FromResult(new ProviderSnapshot(
            Provider: "codex",
            Source: "mock",
            Primary: new UsageWindow(UsedPercent: 34, WindowMinutes: 300, ResetsAt: now.AddHours(2)),
            Secondary: new UsageWindow(UsedPercent: 68, WindowMinutes: 10080, ResetsAt: now.AddDays(4)),
            UpdatedAt: now,
            StatusDescription: "Brak codexbar w PATH – mock data"));
    }
}
