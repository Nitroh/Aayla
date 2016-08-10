// ReSharper disable InconsistentNaming
namespace Nitroh.Aayla.Hearthstone.Data
{
    public class Card
    {
        public string id { get; set; }
        public string set { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string playerClass { get; set; }
        public bool? collectible { get; set; }
        public string rarity { get; set; }
    }
}