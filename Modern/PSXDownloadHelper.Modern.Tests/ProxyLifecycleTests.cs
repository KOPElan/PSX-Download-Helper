using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using PSXDownloadHelper.Modern;
using Xunit;

namespace PSXDownloadHelper.Modern.Tests;

public sealed class ProxyLifecycleTests
{
    [Fact]
    public async Task ServesConcurrentLocalFileRequests()
    {
        using var temp = new TemporaryDirectory();
        var filePath = Path.Combine(temp.Path, "concurrent.pkg");
        var mappingPath = Path.Combine(temp.Path, "local-mappings.json");
        const string payload = "PSX-download-payload-0123456789";
        await File.WriteAllTextAsync(filePath, payload);
        await File.WriteAllTextAsync(mappingPath, JsonSerializer.Serialize(new[]
        {
            new { Url = "http://downloads.example.test/concurrent.pkg", FilePath = filePath }
        }));

        var proxy = new ProxyServer(mappingPath);
        var port = GetFreePort();
        await proxy.StartAsync(IPAddress.Loopback, port);
        try
        {
            var requests = Enumerable.Range(0, 12).Select(async _ =>
            {
                using var client = new TcpClient();
                await client.ConnectAsync(IPAddress.Loopback, port);
                var stream = client.GetStream();
                var request = Encoding.ASCII.GetBytes(
                    "GET http://downloads.example.test/concurrent.pkg HTTP/1.1\r\n" +
                    "Host: downloads.example.test\r\nConnection: close\r\n\r\n");
                await stream.WriteAsync(request);

                using var response = new MemoryStream();
                await stream.CopyToAsync(response).WaitAsync(TimeSpan.FromSeconds(5));
                var text = Encoding.ASCII.GetString(response.ToArray());
                var separator = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                Assert.True(separator >= 0, "Response must contain an HTTP header terminator.");
                Assert.Contains("HTTP/1.1 200 OK", text[..separator], StringComparison.Ordinal);
                Assert.Equal(payload, text[(separator + 4)..]);
            });

            await Task.WhenAll(requests).WaitAsync(TimeSpan.FromSeconds(10));
        }
        finally
        {
            await proxy.StopAsync();
        }
    }

    [Fact]
    public async Task StopAsyncCompletesWhenClientIsWaitingForRequestHeaders()
    {
        var proxy = new ProxyServer();
        var port = GetFreePort();
        await proxy.StartAsync(IPAddress.Loopback, port);

        using var client = new TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, port);

        await proxy.StopAsync().WaitAsync(TimeSpan.FromSeconds(3));

        Assert.False(proxy.IsRunning);
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(), "PSXDownloadHelper.LifecycleTests", Guid.NewGuid().ToString("N"));

        public TemporaryDirectory() => Directory.CreateDirectory(Path);

        public void Dispose()
        {
            try { Directory.Delete(Path, true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
