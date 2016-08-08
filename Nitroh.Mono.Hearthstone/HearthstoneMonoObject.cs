namespace Nitroh.Mono.Hearthstone
{
    public abstract class HearthstoneMonoObject
    {
        protected const string MapKeySlotsString = @"keySlots";
        protected const string MapValueSlotsString = @"valueSlots";
        protected const string ListItemsString = @"_items";
        protected const string ListSizeString = @"_size";

        protected MonoObject MonoObject;

        public bool Valid => MonoObject != null;

        protected HearthstoneMonoObject(MonoObject monoObject)
        {
            MonoObject = monoObject;
        }
    }
}