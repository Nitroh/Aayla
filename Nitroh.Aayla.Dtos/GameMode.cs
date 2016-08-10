namespace Nitroh.Aayla.Dtos
{
    public enum GameMode
    {
        Standard,
        Wild
    }

    public static class GameModeExtensions
    {
        public static GameMode GetGameMode(this Set set)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (set)
            {
                case Set.CurseOfNaxxramas:
                case Set.GoblinsVsGnomes:
                    return GameMode.Wild;
                default:
                    return GameMode.Standard;
            }
        }
    }
}
