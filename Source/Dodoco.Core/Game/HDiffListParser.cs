namespace Dodoco.Core.Game;

using Dodoco.Core.Serialization;
using Dodoco.Core.Serialization.Json;

using System.Text;

public class HDiffListParser {

    private static readonly IFormatSerializer serializer = new JsonSerializer();

    public static GameHDiffFilesEntry Parse(string content) {

        return serializer.Deserialize<GameHDiffFilesEntry>(content);

    }

    public static List<GameHDiffFilesEntry> ParseAll(Stream stream) {

        List<GameHDiffFilesEntry> result = new List<GameHDiffFilesEntry>();

        using (var streamReader = new StreamReader(stream, Encoding.UTF8)) {

            string? line = string.Empty;

            while ((line = streamReader.ReadLine()) != null) {

                result.Add(Parse(line));

            }

        }

        return result;

    }

}