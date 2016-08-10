namespace Nitroh.Aayla.Dtos
{
    public enum Set
    {
        None,
        Basic,
        Classic,
        Reward,
        Promotion,
        CurseOfNaxxramas,
        GoblinsVsGnomes,
        BlackrockMountain,
        TheGrandTournament,
        LeagueOfExplorers,
        WhispersOfTheOldGods,
        OneNightInKarazhan
    }

    public static class SetExtensions
    {
        public static Set ToSet(this string setString)
        {
            switch (setString)
            {
                case "CORE":
                    return Set.Basic;
                case "EXPERT1":
                    return Set.Classic;
                case "REWARD":
                    return Set.Reward;
                case "PROMO":
                    return Set.Promotion;
                case "NAXX":
                    return Set.CurseOfNaxxramas;
                case "GVG":
                    return Set.GoblinsVsGnomes;
                case "BRM":
                    return Set.BlackrockMountain;
                case "TGT":
                    return Set.TheGrandTournament;
                case "LOE":
                    return Set.LeagueOfExplorers;
                case "OG":
                    return Set.WhispersOfTheOldGods;
                case "KARA":
                    return Set.OneNightInKarazhan;
                default:
                    return Set.None;
            }
        }

        public static string GetSetString(this Set set)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (set)
            {
                case Set.Basic:
                    return "Basic";
                case Set.Classic:
                    return "Classic";
                case Set.Reward:
                    return "Reward";
                case Set.Promotion:
                    return "Promotion";
                case Set.CurseOfNaxxramas:
                    return "The Curse of Naxxramas";
                case Set.GoblinsVsGnomes:
                    return "Goblins vs Gnomes";
                case Set.BlackrockMountain:
                    return "Blackrock Mountain";
                case Set.TheGrandTournament:
                    return "The Grand Tournament";
                case Set.LeagueOfExplorers:
                    return "League of Explorers";
                case Set.WhispersOfTheOldGods:
                    return "Whispers of the Old Gods";
                case Set.OneNightInKarazhan:
                    return "One Night in Karazhan";
                default:
                    return "None";
            }
        }
    }
}
