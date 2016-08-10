using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Nitroh.Aayla.Dtos;
using CardDto = Nitroh.Aayla.Dtos.Card;

namespace Nitroh.Aayla.Hearthstone.Data
{
    public class CardRepository
    {
        private List<Card> _cards;

        private List<Card> CollectibleCards => _cards.Where(x => x.collectible ?? false).ToList();

        public bool Valid { get; }

        public CardRepository()
        {
            _cards = new List<Card>();
            Valid = LoadJsonData();
        }

        public IEnumerable<CardDto> GetAllCards()
        {
            var result = new List<CardDto>();
            if (!Valid) return result;
            CollectibleCards.ForEach(x => result.Add(ConvertCard(x)));
            return result.Where(x => x.Type != Type.None).ToList();
        }

        private static CardDto ConvertCard(Card card)
        {
            var result = new CardDto
            {
                Id = card.id,
                Name = card.name,
                Type = card.type.ToType(),
                Rarity = card.rarity.ToRarity(),
                Class = card.playerClass.ToClass(),
                Set = card.set.ToSet()
            };
            result.MaxCount = result.Rarity.GetMaxCount();
            result.SetString = result.Set.GetSetString();
            result.GameMode = result.Set.GetGameMode();
            return result;
        }

        #region Json Loading
        //TODO: Detect updates!
        private bool LoadJsonData()
        {
            var fileInfo = new FileInfo(Constants.CardsJsonFileName);
            if(!fileInfo.Exists) DownloadJsonFile();

            fileInfo = new FileInfo(Constants.CardsJsonFileName);
            if (!fileInfo.Exists) return false;

            string jsonString;
            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    jsonString = streamReader.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(jsonString)) return false;
            _cards = JsonConvert.DeserializeObject<List<Card>>(jsonString);
            return _cards.Count > 0;
        }

        private static void DownloadJsonFile()
        {
            var webClient = new WebClient();
            webClient.DownloadFile(Constants.CardsJsonUrl, Constants.CardsJsonFileName);
        }
        #endregion
    }
}