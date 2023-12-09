using System.Collections;
using UnityEngine;

public abstract class DelayedCommand : MonoCommand {

    [SerializeField, Range(0, 3600), Header("Delay in seconds")]
    float delayDuration = 1;

    public override void Execute()
    {
        StartCoroutine(Run());
    }
    IEnumerator Run()
    {
        yield return new WaitForSeconds(delayDuration);

        DelayedExecute();
    }

    protected abstract void DelayedExecute();
}
