using mini_hw_2.Application.Services;

namespace mini_hw_2.Application.Interfaces;

public interface IStatisticsService
{
    public Task<ZooStatisticsDto> CollectStatisticsAsync();
}