namespace Interface.Json;

public interface IStreamLineReader
{
    IAsyncEnumerable<string> ReadLine(
        Stream stream,
        CancellationToken cancellationToken = default);
}
