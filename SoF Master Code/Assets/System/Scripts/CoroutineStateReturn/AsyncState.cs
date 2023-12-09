/// <summary>
/// Provides the state of an async request
/// </summary>
public abstract class AsyncState : Yieldable {
    public abstract IEventListener EventListener { get; }

    /// overrides Yieldable
    public override abstract void Reset();
    public override abstract bool isDone { get; }

    #region State information
    /// <summary>
    /// returns a value between 0.0f and 1.0f; 1.0f being completed
    /// </summary>
    public abstract float progress { get; }
    /// <summary>
    /// if there is an error that happened
    /// </summary>
    public abstract bool isError { get; }
    /// <summary>
    /// return message, can be error message if isError==true
    /// </summary>
    public abstract string message { get; }
    #endregion
}