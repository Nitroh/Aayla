using System.Linq;

namespace Nitroh.Mono.Hearthstone
{
    public class GameManager : HearthstoneMonoObject
    {
        private const string GameTypeString = @"m_gameType";
        private const string FormatTypeString = @"m_formatType"; 
        private const string SpectatorString = @"m_spectator";

        public GameManager(MonoObject monoObject) : base(monoObject) { }

        public int GameType => MonoObject?[GameTypeString]?.TryGetEnum() ?? -1;

        public int FormatType => MonoObject?[FormatTypeString]?.TryGetEnum() ?? -1;

        public bool Spectating => MonoObject?[SpectatorString]?.TryGetBool() ?? false;
    }
}