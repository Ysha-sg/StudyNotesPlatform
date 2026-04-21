using System.Diagnostics;
using System.Net.Sockets;

namespace StudyNotesPlatform.Services;

public sealed class ViteDevServerService : IHostedService, IDisposable
{
    private const int VitePort = 5173;

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ViteDevServerService> _logger;
    private Process? _process;

    public ViteDevServerService(IWebHostEnvironment environment, ILogger<ViteDevServerService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return Task.CompletedTask;
        }

        if (IsPortOpen("127.0.0.1", VitePort))
        {
            _logger.LogInformation("Vue dev server already listens on http://localhost:{Port}", VitePort);
            return Task.CompletedTask;
        }

        var frontendPath = Path.Combine(_environment.ContentRootPath, "vue-frontend");
        var nodePath = Path.Combine(frontendPath, "node_modules", "node", "bin", "node.exe");
        var vitePath = Path.Combine(frontendPath, "node_modules", "vite", "bin", "vite.js");

        if (!File.Exists(nodePath) || !File.Exists(vitePath))
        {
            _logger.LogWarning(
                "Vue dev server was not started. Install frontend dependencies in {FrontendPath}.",
                frontendPath);
            return Task.CompletedTask;
        }

        var nodeDirectory = Path.GetDirectoryName(nodePath)!;
        var startInfo = new ProcessStartInfo
        {
            FileName = nodePath,
            Arguments = $"\"{vitePath}\" --host 127.0.0.1",
            WorkingDirectory = frontendPath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.Environment["PATH"] = $"{nodeDirectory};{startInfo.Environment["PATH"]}";

        _process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        _process.OutputDataReceived += (_, args) => LogViteLine(args.Data, isError: false);
        _process.ErrorDataReceived += (_, args) => LogViteLine(args.Data, isError: true);
        _process.Exited += (_, _) =>
        {
            if (_process?.ExitCode != 0)
            {
                _logger.LogWarning("Vue dev server stopped with exit code {ExitCode}", _process?.ExitCode);
            }
        };

        if (_process.Start())
        {
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            _logger.LogInformation("Vue dev server is starting on http://localhost:{Port}", VitePort);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_process is { HasExited: false })
        {
            try
            {
                _process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
            }
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _process?.Dispose();
    }

    private static bool IsPortOpen(string host, int port)
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(host, port);
            return connectTask.Wait(TimeSpan.FromMilliseconds(300)) && client.Connected;
        }
        catch
        {
            return false;
        }
    }

    private void LogViteLine(string? line, bool isError)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        if (isError)
        {
            _logger.LogWarning("{Line}", line);
        }
        else
        {
            _logger.LogInformation("{Line}", line);
        }
    }
}
