using System.Runtime.CompilerServices;
using System.Text;
using Interface.Json;

namespace Implementation.Json.Reader;

public class StreamLineReader : IStreamLineReader
{
    public async IAsyncEnumerable<string> ReadLine(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var sr = new StreamReader(stream, Encoding.UTF8);
        while (!sr.EndOfStream)
        {
            var line = await sr.ReadLineAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            
            yield return line ?? string.Empty;
        }
    }
}
