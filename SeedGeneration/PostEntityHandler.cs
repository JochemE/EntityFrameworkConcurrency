using System.Diagnostics;
using Polly;

namespace SeedGeneration;

internal static class PostEntityHandler
{
    internal static async Task<IResult> Handle(ILogger logger)
    {
        var instanceId = Environment.GetEnvironmentVariable("OTEL_RESOURCE_ATTRIBUTES")?
            .Split("=")
            .LastOrDefault() ?? "unknown";

        var stopwatch = Stopwatch.StartNew();

        var result = await PolicyRegistry
            .DatabasePolicy
            .ExecuteAndCaptureAsync(async () =>
            {
                await using var db = new ApplicationDatabaseContext();
                await using var dbContextTransaction = await db.Database.BeginTransactionAsync();
                
                var seed = (await db.Seeds.FindAsync("EntitySeedNumber"))!;

                seed.Number++;
                await db.SaveChangesAsync();
                
                await db.DomainEntities.AddAsync(new DomainEntity
                {
                    Number = $"Entity-{seed.Number:00000}",
                    Title = "title",
                    InstanceId = instanceId,
                });
                await db.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
            });

        if (result.Outcome == OutcomeType.Failure)
        {
            logger.LogWarning("Could not create domain entity in {Duration}", stopwatch.Elapsed);
            return Results.Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
            
        }
        
        logger.LogInformation("Created domain entity in {Duration:0.000}", stopwatch.Elapsed.TotalSeconds);
        return Results.Accepted();
    }
}