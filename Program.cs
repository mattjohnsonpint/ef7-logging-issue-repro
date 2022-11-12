using Microsoft.Extensions.Logging;

namespace EFCoreBugDemo;

internal static class Program
{
    public static async Task Main()
    {
        // Set up a localdb instance and context.
        var instance = new SqlInstance<TestDbContext>(builder =>
        {
            // Add a console logger to the db context
            builder.UseLoggerFactory(LoggerFactory.Create(logging => logging.AddConsole()));
            return new TestDbContext(builder.Options);
        });

        await using var database = await instance.Build();
        await using var dbContext = database.NewDbContext();

        // This will work fine.
        dbContext.Add(new TestEntity {Property = "Value"});
        await dbContext.SaveChangesAsync();
        
        try
        {
            // This will error and throw, because there is a unique index and we've already added one with this name.
            dbContext.Add(new TestEntity {Property = "Value"});
            await dbContext.SaveChangesAsync();
        }
        catch
        {
            //  Swallow the exception - we just want to see the log output
        }
    }
}