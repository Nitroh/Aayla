namespace Nitroh.Mono.Hearthstone
{
    public abstract class HearthstoneMonoObject
    {
        protected MonoObject MonoObject;

        public bool Valid => MonoObject != null;

        protected HearthstoneMonoObject(MonoObject monoObject)
        {
            MonoObject = monoObject;
        }
    }
}