using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using PaymentsService.Infrastructure.Background;

namespace PaymentsServiceTests.Infrastructure.Background
{
    public class InboxEventProcessorTests
    {
        [Fact]
        public async Task Processor_StartsAndStopsWithoutException()
        {
            var spMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<InboxEventProcessor>>();
            var processor = new InboxEventProcessor(spMock.Object, loggerMock.Object);
            using var cts = new CancellationTokenSource(100);
            await processor.StartAsync(cts.Token);
            await processor.StopAsync(CancellationToken.None);
        }
    }
}
