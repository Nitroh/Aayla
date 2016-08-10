namespace Nitroh.Aayla.Dtos
{
    public enum Type
    {
        None,
        Minion,
        Spell,
        Weapon
    }

    public static class TypeExtensions
    {
        public static Type ToType(this string typeString)
        {
            switch (typeString)
            {
                case "MINION":
                    return Type.Minion;
                case "SPELL":
                    return Type.Spell;
                case "WEAPON":
                    return Type.Weapon;
                default:
                    return Type.None;
            }
        }
    }
}
