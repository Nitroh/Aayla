namespace Nitroh.Aayla.Dtos
{
    public enum Rarity
    {
        None,
        Free,
        Common,
        Rare,
        Epic,
        Legendary
    }

    public static class RarityExtensions
    {
        public static Rarity ToRarity(this string rarityString)
        {
            switch (rarityString)
            {
                case "FREE":
                    return Rarity.Free;
                case "COMMON":
                    return Rarity.Common;
                case "RARE":
                    return Rarity.Rare;
                case "EPIC":
                    return Rarity.Epic;
                case "LEGENDARY":
                    return Rarity.Legendary;
                default:
                    return Rarity.None;
            }
        }

        public static int GetMaxCount(this Rarity rarity)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (rarity)
            {
                case Rarity.Legendary:
                    return 1;
                case Rarity.None:
                    return 0;
                default:
                    return 2;
            }
        }
    }
}
