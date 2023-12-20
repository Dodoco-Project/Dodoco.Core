namespace Dodoco.Core.Game;

using Dodoco.Core.Serialization;
using Dodoco.Core.Serialization.Json;

using System.Text;

public class DeprecatedFilesListParser {

    public static List<string> ParseAll(Stream stream) {

        List<string> result = new List<string>();

        using (var streamReader = new StreamReader(stream, Encoding.UTF8)) {

            string? line = string.Empty;

            while ((line = streamReader.ReadLine()) != null) {

                result.Add(line);

            }

        }

        return result;

    }

}