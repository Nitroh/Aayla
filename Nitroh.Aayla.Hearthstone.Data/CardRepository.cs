using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace Nitroh.Aayla.Hearthstone.Data
{
    public class CardRepository
    {
        private List<Card> _cards;

        public bool Valid { get; }

        public CardRepository()
        {
            _cards = new List<Card>();
            Valid = LoadJsonData();
        }

        public IEnumerable<Card> GetAllCards() => Valid ? _cards.Where(x => x.collectible.HasValue && x.collectible.Value).ToList() : null;

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