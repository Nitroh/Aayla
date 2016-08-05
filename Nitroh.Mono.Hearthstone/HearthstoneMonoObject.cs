namespace Nitroh.Mono.Hearthstone
{
    public abstract class HearthstoneMonoObject
    {
        protected MonoObject MonoObject;

        protected HearthstoneMonoObject(MonoObject monoObject)
        {
            MonoObject = monoObject;
        }
    }
}