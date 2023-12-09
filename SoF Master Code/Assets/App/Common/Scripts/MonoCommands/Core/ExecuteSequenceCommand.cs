/// <summary>
/// Runs all MonoCommand scripts attached to the same gameobject as this
/// </summary>
public class ExecuteSequenceCommand : MonoCommand {

    public override void Execute()
    {
        ExecuteOthers(gameObject);
    }

    public static void ExecuteOthers(UnityEngine.GameObject target)
    {
        var list = target.GetComponents<MonoCommand>();
        for (int i = 0; i < list.Length; ++i) {
            // get all monocommands on the same gameobject and run them
            if (list[i].IsIgnoreFromAutoCollection()==false) {
                list[i].Execute();
            }
        }
    }
    public override bool IsIgnoreFromAutoCollection()
    {
        return true;
    }
}
