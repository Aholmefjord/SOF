using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Execute commands attached to the same object on Start
/// </summary>
public class StandaloneCommand : MonoCommand {
    // Use this for initialization
    void Start () {
        Execute();
	}

    public override void Execute()
    {
        ExecuteSequenceCommand.ExecuteOthers(gameObject);
    }
    public override bool IsIgnoreFromAutoCollection()
    {
        return true;
    }
}
