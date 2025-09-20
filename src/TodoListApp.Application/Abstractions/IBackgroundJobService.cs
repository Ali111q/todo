namespace TodoListApp.Application.Abstractions;

public interface IBackgroundJobService
{
    Task CleanupExpiredRefreshTokensAsync(CancellationToken cancellationToken = default);
    Task SendOverdueRemindersAsync(CancellationToken cancellationToken = default);
    Task GenerateDailyReportAsync(CancellationToken cancellationToken = default);
    Task GenerateWeeklyReportAsync(CancellationToken cancellationToken = default);
}
