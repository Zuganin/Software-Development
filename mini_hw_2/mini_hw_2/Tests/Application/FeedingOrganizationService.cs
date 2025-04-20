using mini_hw_2.Application.Interfaces;
using mini_hw_2.Application.Services;
using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.FeedingSchedule;
using mini_hw_2.Domain.Events;
using mini_hw_2.Domain.ValueObjects.FeedingSchedule;
using Moq;
using Xunit;

namespace Tests.Application;

public class FeedingOrganizationServiceTests
    {
        private readonly Mock<IFeedingScheduleRepository> _scheduleRepoMock = new();
        private readonly Mock<IEventPublisher> _eventPublisherMock = new();

        private FeedingOrganizationService CreateService() =>
            new(_scheduleRepoMock.Object, _eventPublisherMock.Object);

        [Fact]
        public async Task AddFeedingScheduleAsync_ShouldAddSchedule()
        {
            // Arrange
            var schedule = new FeedingSchedule(Guid.NewGuid(), Guid.NewGuid(), new FeedingTime(DateTime.Now.AddHours(1)), Food.Meat);
            var service = CreateService();

            // Act
            await service.AddFeedingScheduleAsync(schedule);

            // Assert
            _scheduleRepoMock.Verify(r => r.AddAsync(schedule), Times.Once);
        }

        [Fact]
        public async Task RescheduleFeedingAsync_ShouldUpdateScheduleTime()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var schedule = new FeedingSchedule(scheduleId, Guid.NewGuid(), new FeedingTime(DateTime.Now.AddHours(2)), Food.Fishfood);
            var newTime = new FeedingTime(DateTime.Now.AddHours(3));

            _scheduleRepoMock.Setup(r => r.GetByIdAsync(scheduleId)).ReturnsAsync(schedule);

            var service = CreateService();

            // Act
            await service.RescheduleFeedingAsync(scheduleId, newTime);

            // Assert
            Assert.Equal(newTime, schedule.Time);
            _scheduleRepoMock.Verify(r => r.UpdateAsync(schedule), Times.Once);
        }

        [Fact]
        public async Task RescheduleFeedingAsync_ShouldThrow_IfNotFound()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            _scheduleRepoMock.Setup(r => r.GetByIdAsync(scheduleId)).ReturnsAsync((FeedingSchedule)null);
            var service = CreateService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.RescheduleFeedingAsync(scheduleId, new FeedingTime(DateTime.Now.AddHours(1))));
            Assert.Contains("не найдено", ex.Message);
        }

        [Fact]
        public async Task MarkFeedingCompletedAsync_ShouldMarkAndUpdate()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var schedule = new FeedingSchedule(scheduleId, Guid.NewGuid(), new FeedingTime(DateTime.Now.AddMinutes(30)), Food.Grass);
            _scheduleRepoMock.Setup(r => r.GetByIdAsync(scheduleId)).ReturnsAsync(schedule);

            var service = CreateService();

            // Act
            await service.MarkFeedingCompletedAsync(scheduleId);

            // Assert
            Assert.True(schedule.IsCompleted);
            _scheduleRepoMock.Verify(r => r.UpdateAsync(schedule), Times.Once);
        }

        [Fact]
        public async Task GetFeedingSchedulesAsync_ShouldReturnAll()
        {
            // Arrange
            var schedules = new List<FeedingSchedule>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), new FeedingTime(DateTime.Now.AddHours(1)), Food.Meat),
                new(Guid.NewGuid(), Guid.NewGuid(), new FeedingTime(DateTime.Now.AddHours(2)), Food.Grass)
            };

            _scheduleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(schedules);
            var service = CreateService();

            // Act
            var result = await service.GetFeedingSchedulesAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CheckFeedingTimeAsync_ShouldPublishEvent_IfTimeDueAndNotCompleted()
        {
            // Arrange
            var schedule = new FeedingSchedule(Guid.NewGuid(), Guid.NewGuid(), new FeedingTime(DateTime.Now.AddSeconds(-1)), Food.Fishfood);
            var schedules = new List<FeedingSchedule> { schedule };

            _scheduleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(schedules);
            var service = CreateService();

            // Act
            await service.CheckFeedingTimeAsync();

            // Assert
            _eventPublisherMock.Verify(p => p.PublishAsync(It.IsAny<FeedingTimeEvent>()), Times.Once);
        }

        [Fact]
        public async Task CheckFeedingTimeAsync_ShouldNotPublish_IfAlreadyCompleted()
        {
            // Arrange
            var schedule = new FeedingSchedule(Guid.NewGuid(), Guid.NewGuid(), new FeedingTime(DateTime.Now.AddSeconds(-1)), Food.Fishfood);
            schedule.MarkAsCompleted();

            var schedules = new List<FeedingSchedule> { schedule };
            _scheduleRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(schedules);
            var service = CreateService();

            // Act
            await service.CheckFeedingTimeAsync();

            // Assert
            _eventPublisherMock.Verify(p => p.PublishAsync(It.IsAny<FeedingTimeEvent>()), Times.Never);
        }
    }