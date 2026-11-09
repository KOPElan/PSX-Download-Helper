using PSXDownloadHelper.Modern;
using Xunit;

namespace PSXDownloadHelper.Modern.Tests;

public sealed class RuleStoreTests
{
    [Fact]
    public void CdnRewriteUsesMatchingRuleAndPreservesQueryString()
    {
        using var temp = new TemporaryDirectory();
        var store = new CdnRuleStore(Path.Combine(temp.Path, "cdn-rules.json"));
        store.Save([
            new CdnRule
            {
                Name = "General CDN",
                MatchHost = "cdn.example.test",
                TargetHost = "general-mirror.example.test",
                MatchPathPrefix = "/",
                TargetPathPrefix = "/"
            },
            new CdnRule
            {
                Name = "Package CDN",
                MatchHost = "*.example.test",
                TargetHost = "package-mirror.example.test",
                MatchPathPrefix = "/pkg",
                TargetPathPrefix = "/content",
                Priority = 100
            }
        ]);

        var rewritten = store.Rewrite(
            new Uri("http://cdn.example.test/pkg/game.pkg?region=eu"), out var ruleName);

        Assert.Equal("Package CDN", ruleName);
        Assert.Equal("package-mirror.example.test", rewritten.Host);
        Assert.Equal("/content/game.pkg", rewritten.AbsolutePath);
        Assert.Equal("?region=eu", rewritten.Query);
    }

    [Theory]
    [InlineData("http://cdn.example.test/pkg/file.pkg", true)]
    [InlineData("http://sub.example.test/pkg/file.pkg", true)]
    [InlineData("http://example.test/pkg/file.pkg", false)]
    [InlineData("http://cdn.example.test/pkg-extra/file.pkg", false)]
    public void CdnWildcardAndPathPrefixRespectBoundaries(string address, bool expected)
    {
        var rule = new CdnRule
        {
            Name = "Boundary test",
            MatchHost = "*.example.test",
            TargetHost = "mirror.example.test",
            MatchPathPrefix = "/pkg"
        }.NormalizeAndValidate();

        Assert.Equal(expected, rule.IsMatch(new Uri(address)));
    }

    [Theory]
    [InlineData("/../secret")]
    [InlineData("/pkg/../../secret")]
    [InlineData(@"\pkg\secret")]
    public void CdnRuleRejectsPathTraversal(string path)
    {
        var rule = new CdnRule
        {
            Name = "Invalid path",
            MatchHost = "cdn.example.test",
            TargetHost = "mirror.example.test",
            MatchPathPrefix = path
        };

        Assert.Throws<InvalidDataException>(() => rule.NormalizeAndValidate());
    }

    [Fact]
    public void OfflineUrlRewriteRequiresExactPathAndQuery()
    {
        using var temp = new TemporaryDirectory();
        var store = new OfflineUrlStore(Path.Combine(temp.Path, "offline-urls.json"));
        store.Save([
            new OfflineUrlRule
            {
                Name = "EU mirror",
                SourceUrl = "http://downloads.example.test/game.pkg?region=eu",
                TargetUrl = "http://mirror.example.test/files/game.pkg?region=eu",
                Priority = 20
            }
        ]);

        var matched = store.TryRewrite(
            new Uri("http://DOWNLOADS.example.test/game.pkg?region=eu"),
            out var matchedRule, out var target);
        Assert.True(matched);
        Assert.Equal("EU mirror", matchedRule!.Name);
        Assert.Equal("http://mirror.example.test/files/game.pkg?region=eu", target.AbsoluteUri);

        Assert.False(store.TryRewrite(
            new Uri("http://downloads.example.test/game.pkg?region=us"),
            out _, out _));
        Assert.False(store.TryRewrite(
            new Uri("http://downloads.example.test/Game.pkg?region=eu"),
            out _, out _));
    }

    [Fact]
    public void OfflineUrlSaveRejectsDuplicateSourcesIgnoringHostCase()
    {
        using var temp = new TemporaryDirectory();
        var store = new OfflineUrlStore(Path.Combine(temp.Path, "offline-urls.json"));
        var first = new OfflineUrlRule
        {
            Name = "First",
            SourceUrl = "http://downloads.example.test/file.pkg",
            TargetUrl = "http://mirror1.example.test/file.pkg"
        };
        var duplicate = new OfflineUrlRule
        {
            Name = "Duplicate",
            SourceUrl = "http://DOWNLOADS.example.test/file.pkg",
            TargetUrl = "http://mirror2.example.test/file.pkg"
        };

        Assert.Throws<InvalidDataException>(() => store.Save([first, duplicate]));
    }

    [Theory]
    [InlineData("mirror.example.test\r\nX-Injected: yes", "")]
    [InlineData("", "session=abc\r\nX-Injected: yes")]
    [InlineData("", "")]
    public void OfflineUrlRuleRejectsHeaderInjectionAndHttps(
        string hostHeader, string cookie)
    {
        var rule = new OfflineUrlRule
        {
            Name = "Unsafe rule",
            SourceUrl = "https://downloads.example.test/file.pkg",
            TargetUrl = "http://mirror.example.test/file.pkg",
            HostHeader = hostHeader,
            Cookie = cookie
        };

        Assert.Throws<InvalidDataException>(() => rule.NormalizeAndValidate());
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(), "PSXDownloadHelper.RuleTests", Guid.NewGuid().ToString("N"));

        public TemporaryDirectory() => Directory.CreateDirectory(Path);

        public void Dispose()
        {
            try { Directory.Delete(Path, true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}
