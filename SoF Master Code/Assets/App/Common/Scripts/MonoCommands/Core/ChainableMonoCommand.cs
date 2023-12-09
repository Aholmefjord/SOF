using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChainableMonoCommand : MonoCommand {
    [SerializeField, Tooltip("The next command to execute after this coroutine command")]
    protected MonoCommand nextCommand;

    public override void Execute()
    {
        try {
            StartCoroutine(Run());
        } catch (System.Exception e) {
            // open error overlay
            Debug.LogException(e);
        }
    }

    protected abstract IEnumerator Run();

    public bool ExecuteNextCommand()
    {
        if (nextCommand == null) {
            return false;
        }else {
            nextCommand.Execute();
            return true;
        }
    }
}
