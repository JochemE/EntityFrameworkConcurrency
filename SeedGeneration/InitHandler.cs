namespace SeedGeneration;

internal static class InitHandler
{
    internal static async Task<IResult> Handle()
    {
        await using var db = new ApplicationDatabaseContext();

        await db.Database.EnsureCreatedAsync();

        var seed = await db.Seeds.FindAsync("EntitySeedNumber");
        if (seed != null) return Results.Ok();

        await db.Seeds.AddAsync(new Seed { Id = "EntitySeedNumber", Number = 0 });
        await db.SaveChangesAsync();

        return Results.Ok();
    }
}