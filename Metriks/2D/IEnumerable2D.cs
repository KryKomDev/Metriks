using System.Collections;

namespace Metriks;

public interface IEnumerable2D<out T> : IEnumerable2D 
#if NET9_0_OR_GREATER
    where T : allows ref struct 
#endif
{
    public new IEnumerator<IEnumerable<T>> GetEnumerator();
}

public interface IEnumerable2D {
    public IEnumerator<IEnumerable> GetEnumerator();
}