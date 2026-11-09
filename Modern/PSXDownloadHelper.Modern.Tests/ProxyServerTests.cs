using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using PSXDownloadHelper.Modern;
using Xunit;

namespace PSXDownloadHelper.Modern.Tests;

public sealed class ProxyServerTests
{
    [Fact]
    public async Task ConnectEstablishesOpaqueBidirectionalTunnel()
    {
        using var upstream = new TcpListener(IPAddress.Loopback, 0);
        upstream.Start();
        var upstreamPort = ((IPEndPoint)upstream.LocalEndpoint).Port;
        var upstreamTask = Task.Run(async () =>
        {
            using var accepted = await upstream.AcceptTcpClientAsync();
            var stream = accepted.GetStream();
            var received = new byte[4];
            await stream.ReadExactlyAsync(received);
            await stream.WriteAsync(Encoding.ASCII.GetBytes("pong"));
            accepted.Client.Shutdown(SocketShutdown.Send);
            return Encoding.ASCII.GetString(received);
        });

        var proxy = new ProxyServer();
        await proxy.StartAsync(IPAddress.Loopback, GetFreePort());
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, GetProxyPort(proxy));
            var stream = client.GetStream();
            var request = Encoding.ASCII.GetBytes(
                $"CONNECT 127.0.0.1:{upstreamPort} HTTP/1.1\r\nHost: 127.0.0.1:{upstreamPort}\r\n\r\n");
            await stream.WriteAsync(request);

            var responseHeader = Encoding.ASCII.GetString(await ReadHeaderAsync(stream));
            Assert.Contains("HTTP/1.1 200 Connection Established", responseHeader, StringComparison.Ordinal);

            await stream.WriteAsync(Encoding.ASCII.GetBytes("ping"));
            var responsePayload = new byte[4];
            await stream.ReadExactlyAsync(responsePayload);

            Assert.Equal("pong", Encoding.ASCII.GetString(responsePayload));
            Assert.Equal("ping", await upstreamTask.WaitAsync(TimeSpan.FromSeconds(5)));
        }
        finally
        {
            await proxy.StopAsync();
        }
    }

    [Theory]
    [InlineData("bytes=2-5", "206 Partial Content", "bytes 2-5/10", "2345")]
    [InlineData("bytes=-3", "206 Partial Content", "bytes 7-9/10", "789")]
    public async Task LocalFileMappingServesSingleByteRanges(
        string range, string expectedStatus, string expectedContentRange, string expectedBody)
    {
        using var temp = new TemporaryDirectory();
        var filePath = Path.Combine(temp.Path, "game.pkg");
        var mappingPath = Path.Combine(temp.Path, "local-mappings.json");
        await File.WriteAllTextAsync(filePath, "0123456789");
        await File.WriteAllTextAsync(mappingPath, JsonSerializer.Serialize(new[]
        {
            new { Url = "http://downloads.example.test/game.pkg?region=eu", FilePath = filePath }
        }));

        var proxy = new ProxyServer(mappingPath);
        var proxyPort = GetFreePort();
        await proxy.StartAsync(IPAddress.Loopback, proxyPort);
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, proxyPort);
            var stream = client.GetStream();
            var request = Encoding.ASCII.GetBytes(
                $"GET http://downloads.example.test/game.pkg?region=eu HTTP/1.1\r\nHost: downloads.example.test\r\nRange: {range}\r\nConnection: close\r\n\r\n");
            await stream.WriteAsync(request);
            var response = await ReadToEndAsync(stream);
            var split = response.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            Assert.True(split >= 0, "Proxy response must contain an HTTP header terminator.");

            var header = response[..split];
            var body = response[(split + 4)..];
            Assert.Contains($"HTTP/1.1 {expectedStatus}", header, StringComparison.Ordinal);
            Assert.Contains($"Content-Range: {expectedContentRange}", header, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(expectedBody, body);
        }
        finally
        {
            await proxy.StopAsync();
        }
    }

    [Fact]
    public async Task LocalFileMappingReturns416ForUnsatisfiableRange()
    {
        using var temp = new TemporaryDirectory();
        var filePath = Path.Combine(temp.Path, "game.pkg");
        var mappingPath = Path.Combine(temp.Path, "local-mappings.json");
        await File.WriteAllTextAsync(filePath, "0123456789");
        await File.WriteAllTextAsync(mappingPath, JsonSerializer.Serialize(new[]
        {
            new { Url = "http://downloads.example.test/game.pkg", FilePath = filePath }
        }));

        var proxy = new ProxyServer(mappingPath);
        var proxyPort = GetFreePort();
        await proxy.StartAsync(IPAddress.Loopback, proxyPort);
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, proxyPort);
            var stream = client.GetStream();
            var request = Encoding.ASCII.GetBytes(
                "GET http://downloads.example.test/game.pkg HTTP/1.1\r\nHost: downloads.example.test\r\nRange: bytes=20-30\r\nConnection: close\r\n\r\n");
            await stream.WriteAsync(request);
            var response = await ReadToEndAsync(stream);

            Assert.Contains("HTTP/1.1 416 Range Not Satisfiable", response, StringComparison.Ordinal);
            Assert.Contains("Content-Range: bytes */10", response, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            await proxy.StopAsync();
        }
    }

    private static int GetFreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static async Task<byte[]> ReadHeaderAsync(NetworkStream stream)
    {
        using var buffer = new MemoryStream();
        var one = new byte[1];
        while (await stream.ReadAsync(one) == 1)
        {
            buffer.WriteByte(one[0]);
            var bytes = buffer.ToArray();
            if (bytes.Length >= 4 &&
                bytes[^4] == 13 && bytes[^3] == 10 &&
                bytes[^2] == 13 && bytes[^1] == 10)
                return bytes;
        }
        return buffer.ToArray();
    }

    private static async Task<string> ReadToEndAsync(NetworkStream stream)
    {
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        return Encoding.ASCII.GetString(buffer.ToArray());
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(), "PSXDownloadHelper.Tests", Guid.NewGuid().ToString("N"));

        public TemporaryDirectory() => Directory.CreateDirectory(Path);

        public void Dispose()
        {
            try { Directory.Delete(Path, true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
