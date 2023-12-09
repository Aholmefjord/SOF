using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DelayedTriggerAnimatorCommand : DelayedCommand {

    [SerializeField]
    string triggerName;
    protected override void DelayedExecute()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger(triggerName);
    }
}
