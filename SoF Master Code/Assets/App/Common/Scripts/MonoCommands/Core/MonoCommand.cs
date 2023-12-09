/// <summary>
/// Base class for monobehaviour version of ICommand
/// </summary>
public abstract class MonoCommand : UnityEngine.MonoBehaviour, JULESTech.ICommand {
    public abstract void Execute();
    public virtual bool IsIgnoreFromAutoCollection()
    {
        return false;
    }
}
