using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace GroupSplitter.Cli
{
    class Program
    {
        private const string HistoryFilename = "History.json";
        private const string RoommatesFilename = "Roommates.json";
        private const string IndividualsFilename = "Individuals.json";
        private static readonly JsonSerializer Json = new JsonSerializer
        {
            Formatting = Formatting.Indented
        };

        static void Main()
        {
            var history = ReadHistory();
            var roommates = ReadRoommates();
            var individuals = ReadIndividuals();

            var occurrenceGenerator = new AllCombinationOccurrenceGenerator
            {
                Individuals = new ReadOnlyCollection<string>(individuals),
                ExemptMeetings = roommates
            };

            var scorer = new HistoryBasedScorer(history);

            var splitter = new OccurrencePicker(occurrenceGenerator, scorer);

            var occurrence = splitter.PickBestOccurrence();
            WriteOccurrenceToConsole(occurrence);

            history.Add(occurrence);
            WriteHistory(history);
        }

        private static void WriteOccurrenceToConsole(Occurrence occurrence)
        {
            using (var console = new StreamWriter(Console.OpenStandardOutput()))
            using (var writer = new JsonTextWriter(console))
            {
                Json.Serialize(writer, occurrence);
            }
        }

        private static void WriteHistory(List<Occurrence> history, string filename = HistoryFilename)
        {
            history.Sort((x, y) => -x.Date.CompareTo(y.Date));

            using (var historyFile = new StreamWriter(filename))
            using (var writer = new JsonTextWriter(historyFile))
            {
                Json.Serialize(writer, history);
            }
        }

        private static List<Occurrence> ReadHistory(string filename = HistoryFilename)
        {
            List<Occurrence> history;
            using (var file = File.OpenText(filename))
            using (var reader = new JsonTextReader(file))
            {
                history = Json.Deserialize<List<Occurrence>>(reader);
            }

            return history;
        }

        private static IList<ISet<string>> ReadRoommates(string filename = RoommatesFilename)
        {
            IList<ISet<string>> roommates;
            using (var file = File.OpenText(filename))
            using (var reader = new JsonTextReader(file))
            {
                roommates = Json.Deserialize<IList<ISet<string>>>(reader);
            }

            return roommates;
        }

        private static IList<string> ReadIndividuals(string filename = IndividualsFilename)
        {
            IList<string> individuals;
            using (var file = File.OpenText(filename))
            using (var reader = new JsonTextReader(file))
            {
                individuals = Json.Deserialize<IList<string>>(reader);
            }

            return individuals;
        }
    }
}
