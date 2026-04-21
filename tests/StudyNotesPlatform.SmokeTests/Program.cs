using System.Diagnostics;
using System.Text;
using System.Text.Json;

return await SmokeRunner.RunAsync(args);

internal static class SmokeRunner
{
    public static async Task<int> RunAsync(string[] args)
    {
        var options = ParseOptions(args);
        var projectRoot = ResolveProjectRoot();
        var backendProjectPath = Path.Combine(projectRoot, "StudyNotesPlatform.csproj");

        if (!File.Exists(backendProjectPath))
        {
            Console.Error.WriteLine($"Backend project file was not found: {backendProjectPath}");
            return 1;
        }

        var logDirectory = Path.Combine(projectRoot, "artifacts", "test-logs");
        Directory.CreateDirectory(logDirectory);

        var stdoutLogPath = Path.Combine(logDirectory, "smoke-backend.out.log");
        var stderrLogPath = Path.Combine(logDirectory, "smoke-backend.err.log");

        var backendProcess = StartBackendProcess(projectRoot, backendProjectPath, options.BaseUrl);
        var stdoutLog = new StringBuilder();
        var stderrLog = new StringBuilder();

        backendProcess.OutputDataReceived += (_, eventArgs) =>
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.Data))
            {
                lock (stdoutLog)
                {
                    stdoutLog.AppendLine(eventArgs.Data);
                }
            }
        };

        backendProcess.ErrorDataReceived += (_, eventArgs) =>
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.Data))
            {
                lock (stderrLog)
                {
                    stderrLog.AppendLine(eventArgs.Data);
                }
            }
        };

        try
        {
            backendProcess.Start();
            backendProcess.BeginOutputReadLine();
            backendProcess.BeginErrorReadLine();

            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            await WaitForApiAsync(
                httpClient,
                $"{options.BaseUrl.TrimEnd('/')}/api/notes/statuses",
                options.StartupTimeoutSeconds,
                backendProcess);

            await AssertArrayEndpointAsync(httpClient, $"{options.BaseUrl.TrimEnd('/')}/api/notes/statuses", "notes statuses");
            await AssertArrayEndpointAsync(httpClient, $"{options.BaseUrl.TrimEnd('/')}/api/lookup/all-universities", "universities");
            await AssertArrayEndpointAsync(httpClient, $"{options.BaseUrl.TrimEnd('/')}/api/notes", "catalog notes");

            Console.WriteLine("Smoke tests passed.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Smoke tests failed.");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
        finally
        {
            await StopProcessAsync(backendProcess);
            await File.WriteAllTextAsync(stdoutLogPath, stdoutLog.ToString());
            await File.WriteAllTextAsync(stderrLogPath, stderrLog.ToString());
        }
    }

    private static Process StartBackendProcess(string projectRoot, string backendProjectPath, string baseUrl)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = projectRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(backendProjectPath);
        startInfo.ArgumentList.Add("--urls");
        startInfo.ArgumentList.Add(baseUrl);
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Production";

        return new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };
    }

    private static async Task WaitForApiAsync(HttpClient httpClient, string healthUrl, int timeoutSeconds, Process backendProcess)
    {
        var timeoutAt = DateTime.UtcNow.AddSeconds(timeoutSeconds);
        Exception? lastException = null;

        while (DateTime.UtcNow < timeoutAt)
        {
            if (backendProcess.HasExited)
            {
                throw new InvalidOperationException($"Backend process exited with code {backendProcess.ExitCode} before API was ready.");
            }

            try
            {
                using var response = await httpClient.GetAsync(healthUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            await Task.Delay(1000);
        }

        throw new TimeoutException(
            $"API was not ready within {timeoutSeconds} seconds. " +
            (lastException != null ? $"Last error: {lastException.Message}" : string.Empty));
    }

    private static async Task AssertArrayEndpointAsync(HttpClient httpClient, string url, string endpointName)
    {
        using var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Endpoint '{endpointName}' returned {(int)response.StatusCode}.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);

        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException($"Endpoint '{endpointName}' returned unexpected JSON format.");
        }
    }

    private static async Task StopProcessAsync(Process process)
    {
        if (process.HasExited)
        {
            process.Dispose();
            return;
        }

        try
        {
            process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync();
        }
        catch
        {
            // Best-effort shutdown for smoke runner.
        }
        finally
        {
            process.Dispose();
        }
    }

    private static SmokeOptions ParseOptions(string[] args)
    {
        string? baseUrl = null;
        int startupTimeoutSeconds = 60;

        for (var i = 0; i < args.Length; i++)
        {
            var argument = args[i];
            if (argument.Equals("--base-url", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                baseUrl = args[++i];
                continue;
            }

            if (argument.Equals("--startup-timeout-seconds", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                if (int.TryParse(args[++i], out var parsed) && parsed > 0)
                {
                    startupTimeoutSeconds = parsed;
                }
            }
        }

        return new SmokeOptions(
            BaseUrl: string.IsNullOrWhiteSpace(baseUrl) ? "http://127.0.0.1:5187" : baseUrl,
            StartupTimeoutSeconds: startupTimeoutSeconds);
    }

    private static string ResolveProjectRoot()
    {
        var candidate = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        return candidate;
    }

    private sealed record SmokeOptions(string BaseUrl, int StartupTimeoutSeconds);
}
