using System.Collections;

/// <summary>
/// Used for yieldable objects, especially async task/jobs/coroutines
/// </summary>
public abstract class Yieldable : IEnumerator {
    public abstract void Reset();
    public abstract bool isDone { get; }

    public bool MoveNext()
    {
        return !isDone;
    }
    public object Current {
        get { return null; }
    }
}
