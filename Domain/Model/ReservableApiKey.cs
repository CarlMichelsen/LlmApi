namespace Domain.Model;

public class ReservableApiKey
{
    private readonly Func<ReservableApiKey, Task> unlockAction;
    private bool disposed = false;

    public ReservableApiKey(
        string name,
        string apiKey,
        Func<ReservableApiKey, Task> unlockAction)
    {
        this.Name = name;
        this.ApiKey = apiKey;
        this.unlockAction = unlockAction;
    }

    public string Name { get; init; }

    public string ApiKey { get; set; }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore();
        this.RegisterDispose(false);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (this.disposed)
        {
            return;
        }

        await this.unlockAction(this);

        this.ApiKey = string.Empty; // Make sure the api-key wont be used again.
        this.disposed = true;
    }

    protected virtual void RegisterDispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
    }
}