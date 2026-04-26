namespace WinCodexBar.Models;

public sealed record ProviderSnapshot(
    string Provider,
    string Source,
    UsageWindow? Primary,
    UsageWindow? Secondary,
    DateTimeOffset UpdatedAt,
    string? StatusDescription = null);
