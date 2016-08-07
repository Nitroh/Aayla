using System.Linq;

namespace Nitroh.Mono.Hearthstone
{
    public class HearthstoneExecutable : MonoExecutable
    {
        private const string ImageName = @"Hearthstone";
        private const string GameMgrString = @"GameMgr";
        private const string StaticInstanceString = @"s_instance";
        
        public GameManager GameManager { get; private set; }

        public HearthstoneExecutable() : base(ImageName)
        {
            Initialize();
        }

        private void Initialize()
        {
            GameManager = new GameManager(GetMonoObject(GameMgrString));
        }

        protected override void Refresh()
        {
            base.Refresh();
            Initialize();
        }

        private MonoObject GetMonoObject(string name)
        {
            var monoClass = MonoClasses?.FirstOrDefault(x => x.Name == name);
            var monoField = monoClass?.Fields?.FirstOrDefault(x => x.Name == StaticInstanceString);
            return monoField?.TryGetFieldAsInstance();
        }
    }
}