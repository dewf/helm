namespace Org.Whatever.QtTesting.Support
{
    public interface INativeBuffer<T> : IDisposable
    {
        public Span<T> GetSpan(out int length);
    }
}
