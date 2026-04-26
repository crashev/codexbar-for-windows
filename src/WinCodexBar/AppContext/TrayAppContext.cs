using System.Drawing;
using WinCodexBar.Models;
using WinCodexBar.Providers;

namespace WinCodexBar.AppContext;

public sealed class TrayAppContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly System.Windows.Forms.Timer _refreshTimer;
    private readonly IUsageProvider _provider;
    private readonly ToolStripMenuItem _summaryItem;
    private readonly ToolStripMenuItem _sessionItem;
    private readonly ToolStripMenuItem _weeklyItem;
    private readonly ToolStripMenuItem _updatedItem;
    private readonly ToolStripMenuItem _sourceItem;
    private readonly ToolStripMenuItem _statusItem;

    public TrayAppContext()
    {
        _provider = UsageProviderFactory.CreateDefault();

        _summaryItem = new ToolStripMenuItem("Ładowanie...") { Enabled = false };
        _sessionItem = new ToolStripMenuItem("Session: --") { Enabled = false };
        _weeklyItem = new ToolStripMenuItem("Weekly: --") { Enabled = false };
        _updatedItem = new ToolStripMenuItem("Updated: --") { Enabled = false };
        _sourceItem = new ToolStripMenuItem("Source: --") { Enabled = false };
        _statusItem = new ToolStripMenuItem("Status: --") { Enabled = false };

        var menu = new ContextMenuStrip();
        menu.Items.AddRange([
            _summaryItem,
            _sessionItem,
            _weeklyItem,
            _sourceItem,
            _updatedItem,
            _statusItem,
            new ToolStripSeparator(),
            new ToolStripMenuItem("Refresh now", null, async (_, _) => await RefreshAsync()),
            new ToolStripMenuItem("Exit", null, (_, _) => ExitThread())
        ]);

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Information,
            Text = "WinCodexBar",
            Visible = true,
            ContextMenuStrip = menu
        };

        _refreshTimer = new System.Windows.Forms.Timer { Interval = 60_000 };
        _refreshTimer.Tick += async (_, _) => await RefreshAsync();
        _refreshTimer.Start();

        _ = RefreshAsync();
    }

    protected override void ExitThreadCore()
    {
        _refreshTimer.Stop();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        base.ExitThreadCore();
    }

    private async Task RefreshAsync()
    {
        try
        {
            var snapshot = await _provider.GetSnapshotAsync(CancellationToken.None);
            Render(snapshot);
        }
        catch (Exception ex)
        {
            _summaryItem.Text = "Błąd odczytu usage";
            _statusItem.Text = $"Status: {Trim(ex.Message)}";
            _notifyIcon.Text = "WinCodexBar: błąd odczytu";
        }
    }

    private void Render(ProviderSnapshot snapshot)
    {
        var session = FormatWindow(snapshot.Primary);
        var weekly = FormatWindow(snapshot.Secondary);

        _summaryItem.Text = $"{snapshot.Provider.ToUpperInvariant()} usage";
        _sessionItem.Text = $"Session: {session}";
        _weeklyItem.Text = $"Weekly: {weekly}";
        _sourceItem.Text = $"Source: {snapshot.Source}";
        _updatedItem.Text = $"Updated: {snapshot.UpdatedAt.LocalDateTime:yyyy-MM-dd HH:mm:ss}";
        _statusItem.Text = $"Status: {snapshot.StatusDescription ?? "ok"}";

        _notifyIcon.Text = Trim($"{snapshot.Provider}: S {session} | W {weekly}");
    }

    private static string FormatWindow(UsageWindow? window)
    {
        if (window is null)
        {
            return "n/a";
        }

        var reset = window.ResetsAt.HasValue
            ? $", reset {window.ResetsAt.Value.LocalDateTime:MM-dd HH:mm}"
            : string.Empty;

        return $"{window.RemainingPercent:0.#}% left{reset}";
    }

    private static string Trim(string value)
    {
        return value.Length > 63 ? value[..63] : value;
    }
}
