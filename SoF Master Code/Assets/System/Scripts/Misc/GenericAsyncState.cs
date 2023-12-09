using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAsyncState : AsyncState {
    protected bool _isDone = false;
    protected bool _isError = false;
    protected float _progress = 0.0f;
    protected string _message = "";
    protected IEventListener _events = null;

    public GenericAsyncState(IEventListener events) // dependancy injection:ctor
    {
        _events = events;
    }

    public void SetProgress(float input)
    {
        _progress = Mathf.Clamp01(input);
        if (_events.Contains("OnProgressUpdate"))
            _events.Call<Action<float>>("OnProgressUpdate")(input);
    }
    public void OnEnd(bool isSuccess, string message)
    {
        _isDone = true;
        _isError = !isSuccess;
        _message = message;

        if (_events.Contains("OnFinish"))
            _events.Call<Action<bool, string>>("OnFinish")(isSuccess, message);
    }

    public override bool isDone {
        get {
            return _isDone;
        }
    }
    public override float progress {
        get {
            return _progress;
        }
    }
    public override bool isError {
        get {
            return _isError;
        }
    }
    public override string message {
        get {
            return _message;
        }
    }
    public override IEventListener EventListener {
        get {
            return _events;
        }
    }

    public override void Reset()
    {
        _isDone = false;
        _isError = false;
        _progress = 0.0f;
        _message = "";
        _events = null;
    }
}
