namespace Nitroh.Aayla.Dtos
{
    public enum Class
    {
        None,
        Druid,
        Hunter,
        Mage,
        Paladin,
        Priest,
        Rogue,
        Shaman,
        Warlock,
        Warrior,
        Neutral
    }

    public static class ClassExtensions
    {

        public static Class ToClass(this string classString)
        {
            switch (classString)
            {
                case "DRUID":
                    return Class.Druid;
                case "HUNTER":
                    return Class.Hunter;
                case "MAGE":
                    return Class.Mage;
                case "PALADIN":
                    return Class.Paladin;
                case "PRIEST":
                    return Class.Priest;
                case "ROGUE":
                    return Class.Rogue;
                case "SHAMAN":
                    return Class.Shaman;
                case "WARLOCK":
                    return Class.Warlock;
                case "WARRIOR":
                    return Class.Warrior;
                case "NEUTRAL":
                    return Class.Neutral;
                default:
                    return Class.None;
            }
        }
    }
}
