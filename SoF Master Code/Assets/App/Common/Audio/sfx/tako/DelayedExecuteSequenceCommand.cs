/// <summary>
/// Runs all MonoCommand scripts attached to the same gameobject as this
/// </summary>
public class DelayedExecuteSequenceCommand : DelayedCommand {

    protected override void DelayedExecute()
    {
        // doing this to avoid diamond inheritange problem
        // MonoCommand->DelayedCommand
        // MonoCommand->ExecuteSequenceCommand
        // DelayedExecuteSequenceCommand = ExecuteSequenceCommand+DelayedCommand;
        ExecuteSequenceCommand.ExecuteOthers(gameObject);
    }
    public override bool IsIgnoreFromAutoCollection()
    {
        return true;
    }
}
