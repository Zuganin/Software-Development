using mini_hw_2.Domain.Entities;
using mini_hw_2.Domain.Entities.FeedingSchedule;
using mini_hw_2.Domain.ValueObjects.FeedingSchedule;

namespace Tests.Domain.Entities;

public class FeedingScheduleTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var animalId = Guid.NewGuid();
            var time = new FeedingTime(DateTime.Now.AddHours(2));
            var food = Food.Meat;

            // Act
            var schedule = new FeedingSchedule(id, animalId, time, food);

            // Assert
            Assert.Equal(id, schedule.Id);
            Assert.Equal(animalId, schedule.AnimalId);
            Assert.Equal(time, schedule.Time);
            Assert.Equal(food, schedule.FoodType);
            Assert.False(schedule.IsCompleted);
        }

        [Fact]
        public void Reschedule_ShouldChangeTime_IfNotCompleted()
        {
            var schedule = CreateDefaultSchedule();
            var newTime = new FeedingTime(DateTime.Now.AddHours(5));

            schedule.Reschedule(newTime);

            Assert.Equal(newTime, schedule.Time);
        }

        [Fact]
        public void Reschedule_ShouldThrow_IfAlreadyCompleted()
        {
            var schedule = CreateDefaultSchedule();
            schedule.MarkAsCompleted();

            var newTime = new FeedingTime(DateTime.Now.AddHours(3));

            var exception = Assert.Throws<InvalidOperationException>(() => schedule.Reschedule(newTime));
            Assert.Equal("Нельзя изменить расписание уже выполненного кормления", exception.Message);
        }

        [Fact]
        public void MarkAsCompleted_ShouldSetIsCompletedToTrue()
        {
            var schedule = CreateDefaultSchedule();

            schedule.MarkAsCompleted();

            Assert.True(schedule.IsCompleted);
        }

        [Fact]
        public void MarkAsCompleted_ShouldThrow_IfAlreadyCompleted()
        {
            var schedule = CreateDefaultSchedule();
            schedule.MarkAsCompleted();

            var exception = Assert.Throws<InvalidOperationException>(() => schedule.MarkAsCompleted());
            Assert.Equal("Кормление уже выполнено", exception.Message);
        }

        private FeedingSchedule CreateDefaultSchedule()
        {
            return new FeedingSchedule(Guid.NewGuid(), Guid.NewGuid(), new FeedingTime(DateTime.Now.AddHours(1)), Food.Meat);
        }
    }