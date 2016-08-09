using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nitroh.Mono.Hearthstone;

namespace Nitroh.Aayla.CollectionCsvScript
{
    public static class Program
    {
        private const string SolutionItemsDirectoryName = @"SolutionItems";
        private const string CardsCsvFileName = @"cards.csv";
        private const string OutputCsvFileName = @"collection.csv";

        public static void Main(string[] args)
        {
            var cards = GetCards();
            if (cards == null || cards.Count == 0)
            {
                //TODO: error here
                return;
            }

            var hearthstone = new HearthstoneExecutable();
            if (!hearthstone.Running)
            {
                //TODO: error here
                return;
            }

            var cardStacks = hearthstone.NetCache?.Map?.Collection?.CardStacks;
            if (cardStacks == null)
            {
                //TODO: error here
                return;
            }

            var results = cards.Select(card => new CollectionCard {Id = card.Key, Name = card.Value, Count = 0, GoldenCount = 0}).ToList();
            foreach (var cardStack in cardStacks)
            {
                var id = cardStack.CardDefinition.Name;
                var index = results.FindIndex(x => x.Id == id);
                if (index == -1) throw new Exception("Should not see this");
                var name = results[index].Name;
                var count = cardStack.CardDefinition.Premium == 0 ? cardStack.Count : results[index].Count;
                var goldenCount = cardStack.CardDefinition.Premium == 1 ? cardStack.Count : results[index].GoldenCount;
                results[index] = new CollectionCard { Id = id, Name = name, Count = count, GoldenCount = goldenCount };
            }

            OutputCollection(results);
        }

        private static void OutputCollection(IEnumerable<CollectionCard> collection)
        {
            var fileInfo = new FileInfo(OutputCsvFileName);
            if(fileInfo.Exists) fileInfo.Delete();
            var sb = new StringBuilder();
            sb.AppendLine("\"Name\",\"Count\",\"GoldenCount\"");
            foreach (var card in collection)
            {
                sb.AppendLine($"\"{card.Name}\",\"{card.Count}\",\"{card.GoldenCount}\"");
            }
            File.WriteAllText(fileInfo.FullName, sb.ToString());
        }

        private static Dictionary<string, string> GetCards()
        {
            var result = new Dictionary<string, string>();
            var currentDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            var baseDirectoryInfo = currentDirectoryInfo.Parent?.Parent?.Parent;
            var solutionItemsDirectoryInfo = baseDirectoryInfo?.GetDirectories().FirstOrDefault(x => x.Name == SolutionItemsDirectoryName);
            var csvFileInfo = solutionItemsDirectoryInfo?.GetFiles().FirstOrDefault(x => x.Name == CardsCsvFileName);
            if (csvFileInfo == null) return null;

            var csvData = new List<string>();
            using (var fileStream = new FileStream(csvFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    var line = streamReader.ReadLine();
                    while (line != null && !streamReader.EndOfStream)
                    {
                        csvData.Add(line);
                        line = streamReader.ReadLine();
                    }
                }
            }

            var separators = new[] {"\",\""};
            for (var index = 1; index < csvData.Count; index++)
            {
                var line = csvData[index];
                var splitLine = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                bool collectible;
                var valid = bool.TryParse(splitLine[2].Replace("\"", ""), out collectible);
                if (!valid || !collectible) continue;
                result.Add(splitLine[0].Replace("\"", ""), splitLine[1].Replace("\"", ""));
            }
            return result;
        }
    }
}
