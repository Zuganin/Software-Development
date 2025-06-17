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
    public class InboxKafkaConsumerTests
    {
        [Fact]
        public async Task Consumer_StartsAndStopsWithoutException()
        {
            var spMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<InboxKafkaConsumer>>();
            var consumer = new InboxKafkaConsumer(spMock.Object, loggerMock.Object);
            using var cts = new CancellationTokenSource(100);
            await consumer.StartAsync(cts.Token);
            await consumer.StopAsync(CancellationToken.None);
        }
    }
}
