namespace Nitroh.Aayla.Dtos
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public Set Set { get; set; }
        public string SetString { get; set; }
        public Rarity Rarity { get; set; }
        public Class Class { get; set; }
        public GameMode GameMode { get; set; }
        public int MaxCount { get; set; }

        public static string GetCsvHeaderLine() => "\"Id\",\"Name\",\"Type\",\"Set\",\"Rarity\",\"Class\",\"GameMode\",\"MaxCount\"";

        public string ToCsvLine() => $"\"{Id}\",\"{Name}\",\"{Type}\",\"{SetString}\",\"{Rarity}\",\"{Class}\",\"{GameMode}\",\"{MaxCount}\"";
    }
}
