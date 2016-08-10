using Card = Nitroh.Aayla.Dtos.Card;

namespace Nitroh.Aayla.CollectionCsvScript
{
    public class CollectionCard
    {
        public Card Card { get; set; }
        public int Count { get; set; }
        public int GoldenCount { get; set; }

        public static string GetCsvHeaderLine() => Card.GetCsvHeaderLine() + ",Count,GoldenCount";

        public string ToCsvLine() => Card.ToCsvLine() + $",\"{Count}\",\"{GoldenCount}\"";
    }
}
