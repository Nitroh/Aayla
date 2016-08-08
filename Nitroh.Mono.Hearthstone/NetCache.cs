using System.Collections.Generic;
using System.Linq;

namespace Nitroh.Mono.Hearthstone
{
    public class NetCache : HearthstoneMonoObject
    {
        private const string NetCacheMapString = @"m_netCache";

        public NetCache(MonoObject monoObject) : base(monoObject)
        {
            _map = null;
        }

        private MonoObject MapMonoObject => MonoObject?[NetCacheMapString]?.TryGetMonoObjectFromPointer();
        private NetCacheMap _map;
        public NetCacheMap Map => _map ?? (_map = new NetCacheMap(MapMonoObject));
    }

    public class NetCacheMap : HearthstoneMonoObject
    {
        private const string NetCacheCollectionString = @"NetCacheCollection";

        public NetCacheMap(MonoObject monoObject) : base(monoObject)
        {
            _collection = null;
        }

        private Dictionary<string, MonoObject> ValueSlots => MonoObject?[MapValueSlotsString]?.TryGetArrayOfObjects();

        private NetCacheCollection _collection;
        public NetCacheCollection Collection => _collection ?? (_collection = new NetCacheCollection(ValueSlots[NetCacheCollectionString]));
    }

    public class NetCacheCollection : HearthstoneMonoObject
    {
        private const string CardStackString = @"<Stacks>k__BackingField";

        public NetCacheCollection(MonoObject monoObject) : base(monoObject)
        {
            _cardStacks = null;
        }

        private MonoObject Stacks => MonoObject?[CardStackString]?.TryGetMonoObjectFromPointer();
        private IEnumerable<MonoObject> StacksItems => Stacks?[ListItemsString]?.TryGetArrayOfPointers();

        private IEnumerable<NetCacheCardStack> _cardStacks;
        public IEnumerable<NetCacheCardStack> CardStacks => _cardStacks ?? (_cardStacks = StacksItems.Select(x => new NetCacheCardStack(x)));
    }

    public class NetCacheCardStack : HearthstoneMonoObject
    {
        private const string CountString = @"<Count>k__BackingField";
        private const string DefString = @"<Def>k__BackingField";

        public NetCacheCardStack(MonoObject monoObject) : base(monoObject)
        {
            _cardDefinition = null;
        }

        public int Count => MonoObject?[CountString]?.TryGetInt() ?? 0;

        private MonoObject Def => MonoObject?[DefString]?.TryGetMonoObjectFromPointer();
        private NetCacheCardDefinition _cardDefinition;
        public NetCacheCardDefinition CardDefinition => _cardDefinition ?? (_cardDefinition = new NetCacheCardDefinition(Def));
    }

    public class NetCacheCardDefinition : HearthstoneMonoObject
    {
        private const string NameString = @"<Name>k__BackingField";
        private const string PremiumString = @"<Premium>k__BackingField";

        public NetCacheCardDefinition(MonoObject monoObject) : base(monoObject) { }

        public int Premium => MonoObject?[PremiumString]?.TryGetInt() ?? 0;

        public string Name => MonoObject?[NameString]?.TryGetString();
    }
}
