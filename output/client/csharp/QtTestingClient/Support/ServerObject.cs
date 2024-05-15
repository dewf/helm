namespace Org.Whatever.QtTesting.Support
{
    internal abstract class ServerObject : ServerResource
    {
        protected ServerObject(int id) : base(id)
        {
        }
        protected override void NativePush()
        {
            NativeMethods.pushInstance(Id, false);
        }
    }
}
