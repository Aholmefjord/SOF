using UnityEngine;

/// <summary>
/// TOM: using stuff that has already been built
/// </summary>
public class LoadSceneCommand : MonoCommand {
    [SerializeField]
    protected string targetSceneName = "SplashScreen";

    public override void Execute()
    {
        // TOM: TODO: !!NOTE!! change this to ensure reusability
        MainNavigationController.DoLoad(targetSceneName);
    }
}
