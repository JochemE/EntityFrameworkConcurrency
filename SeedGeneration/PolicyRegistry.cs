using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

namespace SeedGeneration;

public static class PolicyRegistry
{
    public static AsyncRetryPolicy DatabasePolicy => Policy
        .Handle<DbUpdateConcurrencyException>()
        .Or<DbUpdateException>()
        // .Or<Exception>()
        .WaitAndRetryAsync(
            retryCount: 10,
            sleepDurationProvider: retry => TimeSpan.FromSeconds(.1 * retry),
            onRetry: (exception, timeSpan, context) =>
            {
                if (exception is DbUpdateConcurrencyException concurrencyException)
                {
                    // expected because of optimistic concurrency
                    var message = concurrencyException.Message;
                }
                
                if (exception is DbUpdateException updateException)
                {
                    // caused by unique key constraint?
                    var message = updateException.Message;
                }
            });
}