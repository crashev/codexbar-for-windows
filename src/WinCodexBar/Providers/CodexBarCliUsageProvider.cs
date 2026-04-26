using System.Diagnostics;
using System.Text.Json;
using WinCodexBar.Models;

namespace WinCodexBar.Providers;

public sealed class CodexBarCliUsageProvider : IUsageProvider
{
    public async Task<ProviderSnapshot> GetSnapshotAsync(CancellationToken cancellationToken)
    {
        var output = await RunCodexBarAsync(cancellationToken);
        return Parse(output);
    }

    private static async Task<string> RunCodexBarAsync(CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "codexbar",
            Arguments = "--provider codex --format json",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        if (!process.Start())
        {
            throw new InvalidOperationException("Nie udało się uruchomić codexbar CLI.");
        }

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"codexbar exited {process.ExitCode}: {stderr}");
        }

        return stdout;
    }

    private static ProviderSnapshot Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;
        var providerNode = root.ValueKind == JsonValueKind.Array
            ? root.EnumerateArray().FirstOrDefault()
            : root;

        if (providerNode.ValueKind == JsonValueKind.Undefined)
        {
            throw new InvalidOperationException("CLI returned empty JSON payload.");
        }

        var provider = providerNode.TryGetProperty("provider", out var providerProp)
            ? providerProp.GetString() ?? "codex"
            : "codex";

        var source = providerNode.TryGetProperty("source", out var sourceProp)
            ? sourceProp.GetString() ?? "unknown"
            : "unknown";

        var updatedAt = DateTimeOffset.Now;
        UsageWindow? primary = null;
        UsageWindow? secondary = null;

        if (providerNode.TryGetProperty("usage", out var usage))
        {
            if (usage.TryGetProperty("updatedAt", out var updatedProp)
                && updatedProp.ValueKind == JsonValueKind.String
                && DateTimeOffset.TryParse(updatedProp.GetString(), out var parsedUpdated))
            {
                updatedAt = parsedUpdated;
            }

            if (usage.TryGetProperty("primary", out var primaryNode))
            {
                primary = ParseWindow(primaryNode);
            }

            if (usage.TryGetProperty("secondary", out var secondaryNode))
            {
                secondary = ParseWindow(secondaryNode);
            }
        }

        string? statusDescription = null;
        if (providerNode.TryGetProperty("status", out var statusNode)
            && statusNode.TryGetProperty("description", out var descNode)
            && descNode.ValueKind == JsonValueKind.String)
        {
            statusDescription = descNode.GetString();
        }

        return new ProviderSnapshot(provider, source, primary, secondary, updatedAt, statusDescription);
    }

    private static UsageWindow? ParseWindow(JsonElement node)
    {
        if (node.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        var usedPercent = node.TryGetProperty("usedPercent", out var usedProp)
            ? usedProp.GetDouble()
            : 0;

        var windowMinutes = node.TryGetProperty("windowMinutes", out var windowProp)
            ? windowProp.GetInt32()
            : 0;

        DateTimeOffset? resetsAt = null;
        if (node.TryGetProperty("resetsAt", out var resetProp)
            && resetProp.ValueKind == JsonValueKind.String
            && DateTimeOffset.TryParse(resetProp.GetString(), out var parsedReset))
        {
            resetsAt = parsedReset;
        }

        return new UsageWindow(usedPercent, windowMinutes, resetsAt);
    }
}
