using System.Net;
using FileAnalysisService.Infrastructure.WordCloud;
using Moq;
using Moq.Protected;

namespace FileAnalysisTest.Infrastructure.WorldCloud
{
    public class QuickChartWordCloudGeneratorTests
    {
        private HttpClient CreateHttpClient(HttpStatusCode status, byte[] content)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<System.Threading.CancellationToken>())
                .ReturnsAsync((HttpRequestMessage req, System.Threading.CancellationToken ct) =>
                {
                    return new HttpResponseMessage(status)
                    {
                        Content = new ByteArrayContent(content)
                    };
                });
            return new HttpClient(handlerMock.Object);
        }

        [Fact]
        public async Task GenerateWordCloudAsync_Saves_File_And_Returns_Path()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var savePath = Path.Combine(tempDir, "cloud.png");
            var httpClient = CreateHttpClient(HttpStatusCode.OK, new byte[] { 1, 2, 3, 4 });

            var generator = new QuickChartWordCloudGenerator(httpClient);

            var result = await generator.GenerateWordCloudAsync("hello world hello", savePath);

            Assert.Equal(savePath, result);
            Assert.True(File.Exists(savePath));
            var fileBytes = await File.ReadAllBytesAsync(savePath);
            Assert.Equal(new byte[] { 1, 2, 3, 4 }, fileBytes);

            File.Delete(savePath);
            Directory.Delete(tempDir);
        }

        [Fact]
        public async Task GenerateWordCloudAsync_Creates_Directory_If_Not_Exists()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var savePath = Path.Combine(tempDir, "cloud.png");
            var httpClient = CreateHttpClient(HttpStatusCode.OK, new byte[] { 5, 6 });

            var generator = new QuickChartWordCloudGenerator(httpClient);

            Assert.False(Directory.Exists(tempDir));
            var result = await generator.GenerateWordCloudAsync("foo bar", savePath);
            Assert.True(Directory.Exists(tempDir));
            Assert.True(File.Exists(savePath));

            File.Delete(savePath);
            Directory.Delete(tempDir);
        }

        [Fact]
        public async Task GenerateWordCloudAsync_Throws_On_Http_Error()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var savePath = Path.Combine(tempDir, "cloud.png");
            var httpClient = CreateHttpClient(HttpStatusCode.BadRequest, Array.Empty<byte>());

            var generator = new QuickChartWordCloudGenerator(httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(() =>
                generator.GenerateWordCloudAsync("fail test", savePath));
        }
    }
}