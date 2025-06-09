using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileAnalysisService.Infrastructure.WordCloud;
using Moq;
using Moq.Protected;
using Xunit;

namespace FileAnalysisTest.Infrastructure.WorldCloud
{
    public class QuickChartWordCloudGeneratorTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly Mock<HttpMessageHandler> _mockHandler;

        public QuickChartWordCloudGeneratorTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
            _mockHandler = new Mock<HttpMessageHandler>();
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        private HttpClient CreateClient()
        {
            return new HttpClient(_mockHandler.Object);
        }

        private void SetupMockResponse(HttpStatusCode status, byte[] content = null, string expectedText = null)
        {
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.AbsoluteUri == "https://quickchart.io/wordcloud" &&
                        (expectedText == null || req.Content.ReadAsStringAsync().GetAwaiter().GetResult().Contains(expectedText))),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = content != null ? new ByteArrayContent(content) : null
                });
        }

        [Fact]
        public async Task GenerateWordCloudAsync_Success_SavesFileAndReturnsPath()
        {
            var imagePath = Path.Combine(_tempDir, "cloud.png");
            var expectedBytes = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            SetupMockResponse(HttpStatusCode.OK, expectedBytes);

            // Ensure the file does not exist before the test
            if (File.Exists(imagePath))
                File.Delete(imagePath);

            var generator = new QuickChartWordCloudGenerator(CreateClient());
            var result = await generator.GenerateWordCloudAsync("test", imagePath);

            Assert.Equal(imagePath, result);
            Assert.True(File.Exists(imagePath));
            var fileBytes = await File.ReadAllBytesAsync(imagePath);
            Assert.Equal(expectedBytes.Length, fileBytes.Length);
            Assert.Equal(expectedBytes, fileBytes);
        }

        [Fact]
        public async Task GenerateWordCloudAsync_CreatesDirectoryIfNeeded()
        {
            var nestedDir = Path.Combine(_tempDir, "a", "b");
            var imagePath = Path.Combine(nestedDir, "cloud.png");
            SetupMockResponse(HttpStatusCode.OK, new byte[] { 5, 6 });

            var generator = new QuickChartWordCloudGenerator(CreateClient());
            await generator.GenerateWordCloudAsync("dirtest", imagePath);

            Assert.True(Directory.Exists(nestedDir));
            Assert.True(File.Exists(imagePath));
        }

        [Fact]
        public async Task GenerateWordCloudAsync_ThrowsOnHttpError()
        {
            var imagePath = Path.Combine(_tempDir, "cloud.png");
            // Setup mock to return 400 Bad Request
            SetupMockResponse(HttpStatusCode.BadRequest, new byte[0]);

            var generator = new QuickChartWordCloudGenerator(CreateClient());

            await Assert.ThrowsAsync<Exception>(() =>
                generator.GenerateWordCloudAsync("test", imagePath));
        }

        [Fact]
        public async Task GenerateWordCloudAsync_ThrowsOnNetworkError()
        {
            var imagePath = Path.Combine(_tempDir, "neterr.png");
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("network"));

            var generator = new QuickChartWordCloudGenerator(CreateClient());
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                generator.GenerateWordCloudAsync("net", imagePath));
            Assert.False(File.Exists(imagePath));
        }

        [Fact]
        public async Task GenerateWordCloudAsync_ThrowsOnIoError()
        {
            // Use a path that is not writable on macOS, e.g. /root/forbidden.png
            var imagePath = "/root/forbidden.png";
            SetupMockResponse(HttpStatusCode.OK, new byte[] { 1 });

            var generator = new QuickChartWordCloudGenerator(CreateClient());
            await Assert.ThrowsAnyAsync<Exception>(() =>
                generator.GenerateWordCloudAsync("io", imagePath));
        }
    }
}