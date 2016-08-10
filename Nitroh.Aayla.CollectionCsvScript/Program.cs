using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nitroh.Aayla.Hearthstone.Data;
using Nitroh.Mono.Hearthstone;
using Card = Nitroh.Aayla.Dtos.Card;

namespace Nitroh.Aayla.CollectionCsvScript
{
    public static class Program
    {
        private const string OutputCsvFileName = @"collection.csv";

        public static void Main(string[] args)
        {
            var cards = GetCards().ToList();
            if (cards.Count == 0)
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

            var results = cards.Select(card => new CollectionCard {Card = card, Count = 0, GoldenCount = 0}).ToList();
            foreach (var cardStack in cardStacks)
            {
                var id = cardStack.CardDefinition.Name;
                var index = results.FindIndex(x => x.Card.Id == id);
                if (index == -1) throw new Exception("Should not see this");
                var count = cardStack.CardDefinition.Premium == 0 ? cardStack.Count : results[index].Count;
                var goldenCount = cardStack.CardDefinition.Premium == 1 ? cardStack.Count : results[index].GoldenCount;
                results[index] = new CollectionCard { Card = results[index].Card, Count = count, GoldenCount = goldenCount };
            }

            OutputCollection(results);
        }

        private static void OutputCollection(IEnumerable<CollectionCard> collection)
        {
            var collectionList = collection as IList<CollectionCard> ?? collection.ToList();

            var fileInfo = new FileInfo(OutputCsvFileName);
            if(fileInfo.Exists) fileInfo.Delete();

            var sb = new StringBuilder();
            sb.AppendLine(CollectionCard.GetCsvHeaderLine());
            collectionList.ToList().ForEach(x => sb.AppendLine(x.ToCsvLine()));
            File.WriteAllText(fileInfo.FullName, sb.ToString());
        }

        private static IEnumerable<Card> GetCards()
        {
            var repo = new CardRepository();
            var cards = repo.GetAllCards().ToList();
            return cards;
        }
    }
}
