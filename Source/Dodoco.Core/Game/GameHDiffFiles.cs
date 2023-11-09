using Dodoco.Core.Serialization;
using Dodoco.Core.Serialization.Json;

namespace Dodoco.Core.Game {

    public class GameHDiffFiles: WritableManagedFile<List<GameHDiffFilesEntry>> {

        public GameHDiffFiles(string gameInstallationDirectory): base(
            "hdifffiles",
            gameInstallationDirectory,
            "hdifffiles.txt"
        ) {}

        public override List<GameHDiffFilesEntry> Read() {

            List<GameHDiffFilesEntry> result = new List<GameHDiffFilesEntry>();
            IFormatSerializer serializer = new JsonSerializer();

            foreach (string line in File.ReadAllLines(this.FullPath)) {

                result.Add(serializer.Deserialize<GameHDiffFilesEntry>(line));

            }

            return result;

        }

        public override void Write(List<GameHDiffFilesEntry> content) {

            throw new NotSupportedException();

        }

    }

}