using System.Runtime.CompilerServices;
using System.Text;
using Interface.Json;

namespace Implementation.Json.Reader;

public class StreamLineReader : IStreamLineReader
{
    public static readonly bool WriteToFile = false;

    public async IAsyncEnumerable<string> ReadLine(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var filename = $"test-stream-data-{DateTime.UtcNow.ToString("dd-MM-yyyy-HH-mm-ss")}.txt";

        using var sr = new StreamReader(stream, Encoding.UTF8);
        while (!sr.EndOfStream)
        {
            var line = await sr.ReadLineAsync(cancellationToken);

            if (WriteToFile)
            {
                this.CreateAndOrAppendTestFile(filename, line ?? string.Empty);
            }
            
            yield return line ?? string.Empty;

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }
        }
    }

    public void CreateAndOrAppendTestFile(string filename, string line)
    {
        var filePath = $"./{filename}";
        using (StreamWriter writer = File.AppendText(filePath))
        {
            Console.WriteLine(line);
            writer.WriteLine(line);
        }
    }
}
