/// <summary>
/// 
/// </summary>
public interface IAsyncRequest {
    /// <summary>
    /// Starts this request. Will return a Yieldable AsyncState object
    /// to observe the state;
    /// </summary>
    /// <returns></returns>
    AsyncState Execute();
    /// <summary>
    /// Stops this async request;
    /// </summary>
    void Stop();
    /// <summary>
    /// Retrieve the AsyncState object;
    /// </summary>
    AsyncState CurrentState { get; }
    /// <summary>
    /// Get a refernece to the eventlistener object
    /// Register to receive events;
    /// </summary>
    IEventListener EventListener { get; }
}
