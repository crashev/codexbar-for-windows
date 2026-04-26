using WinCodexBar.Models;

namespace WinCodexBar.Providers;

public interface IUsageProvider
{
    Task<ProviderSnapshot> GetSnapshotAsync(CancellationToken cancellationToken);
}
