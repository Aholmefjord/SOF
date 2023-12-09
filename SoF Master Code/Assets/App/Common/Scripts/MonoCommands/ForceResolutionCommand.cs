using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceResolutionCommand : MonoCommand {
    [SerializeField]
    int targetWidth = 1280;
    [SerializeField]
    int targetHeight = 720;
    [SerializeField]
    bool fullscreen = true;

    public override void Execute()
    {
        Screen.SetResolution(targetWidth, targetHeight, fullscreen);
    }
}
