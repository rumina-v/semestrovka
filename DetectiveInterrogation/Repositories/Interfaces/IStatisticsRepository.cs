namespace DetectiveInterrogation.Repositories.Interfaces;

public interface IStatisticsRepository
{
    Task<int> GetUsersCountAsync();
    Task<int> GetCasesCountAsync();
    Task<int> GetInterrogationsCountAsync();
}
