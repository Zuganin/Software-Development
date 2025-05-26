using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileAnalysisService.Application.DTOs;
using FileAnalysisService.Application.Interfaces;
using FileAnalysisService.Application.Services;
using FileAnalysisService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FileAnalysisTest.Controllers
{
    public class AnalyzeControllerTests
    {
        [Fact]
        public async Task GetAsync_Returns_Ok_With_Response()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var response = new AnalyzeFileResponse
            {
                FileId = fileId,
                PlagiarismPercent = 42,
                WordCloudPath = "/clouds/wordcloud.png"
            };
            
            var serviceMock = new Mock<IAnalyzeFileService>();
            serviceMock
                .Setup(s => s.AnalyzeAsync(It.Is<AnalyzeFileRequest>(r => r.FileId == fileId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controller = new AnalyzeController(serviceMock.Object);

            // Act
            var result = await controller.GetAsync(fileId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
            serviceMock.Verify(s => s.AnalyzeAsync(It.Is<AnalyzeFileRequest>(r => r.FileId == fileId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_Returns_NotFound_On_FileNotFoundException()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var serviceMock = new Mock<IAnalyzeFileService>();
            serviceMock
                .Setup(s => s.AnalyzeAsync(It.IsAny<AnalyzeFileRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new FileNotFoundException());

            var controller = new AnalyzeController(serviceMock.Object);

            // Act
            var result = await controller.GetAsync(fileId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            serviceMock.Verify(s => s.AnalyzeAsync(It.Is<AnalyzeFileRequest>(r => r.FileId == fileId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}