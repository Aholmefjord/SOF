/// <summary>
/// reset GameState.nextScene value; Used before going "start_menu" scene
/// </summary>
public class ClearGameStateCommand : MonoCommand {
    public override void Execute()
    {
        GameState.nextScene = null;
    }
}
