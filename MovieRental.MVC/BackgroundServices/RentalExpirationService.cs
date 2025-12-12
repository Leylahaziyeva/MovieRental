using MovieRental.BLL.Services.Contracts;

namespace MovieRental.MVC.BackgroundServices
{
    // Background service that automatically expires old rentals every hour
    public class RentalExpirationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RentalExpirationService> _logger;
        private readonly TimeSpan _checkInterval;

        public RentalExpirationService(
            IServiceScopeFactory scopeFactory,
            ILogger<RentalExpirationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _checkInterval = TimeSpan.FromHours(1); 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RentalExpirationService started at {Time}", DateTime.UtcNow);

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExpireOldRentalsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while expiring rentals at {Time}", DateTime.UtcNow);
                }

                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("RentalExpirationService stopping at {Time}", DateTime.UtcNow);
                    break;
                }
            }

            _logger.LogInformation("RentalExpirationService stopped at {Time}", DateTime.UtcNow);
        }

        private async Task ExpireOldRentalsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var rentalService = scope.ServiceProvider.GetRequiredService<IRentalService>();

            try
            {
                var expiredCount = await rentalService.ExpireOldRentalsAsync();

                if (expiredCount > 0)
                {
                    _logger.LogInformation(
                        "Successfully expired {Count} rental(s) at {Time}",
                        expiredCount,
                        DateTime.UtcNow
                    );
                }
                else
                {
                    _logger.LogDebug("No rentals to expire at {Time}", DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to expire rentals at {Time}",
                    DateTime.UtcNow
                );
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RentalExpirationService is stopping...");
            await base.StopAsync(cancellationToken);
        }
    }
}