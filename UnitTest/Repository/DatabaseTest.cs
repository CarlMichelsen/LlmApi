using Implementation.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Repository;

public abstract class DatabaseTest : IDisposable
{
    private readonly SqliteConnection connection;
    private readonly ApplicationContext context;

    protected DatabaseTest()
    {
        this.connection = new SqliteConnection("DataSource=:memory:");
        this.connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlite(this.connection) // Generate a unique in-memory database for each test
            .Options;
        
        this.context = new ApplicationContext(options);
        
        this.context.Database.EnsureCreated();
        this.SeedDatabase();

        this.context.SaveChanges();
    }

    protected ApplicationContext Context => this.context;

    public void Dispose()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();

        this.connection.Close();
        this.connection.Dispose();
    }

    protected virtual void SeedDatabase()
    {
        // Do nothing by default
    }
}
