using System;
using System.Collections;
using UnityEngine;

public class GenericCoroutineRequest : IAsyncRequest {
    protected AsyncState m_State = null;
    protected IEventListener m_Listener = null;
    protected Func<IAsyncRequest, IEnumerator> m_Job = null;
    protected Coroutine _inst = null;

    public GenericCoroutineRequest (AsyncState state, Func<IAsyncRequest, IEnumerator> job)
    {
        m_Job = job;
        m_State = state;
        m_Listener = state.EventListener;
        Action<bool, string> stopCall = (success, msg) => { Stop(); };
        m_Listener.Register("OnFinish", stopCall);
    }

    public AsyncState Execute()
    {
        if (_inst != null) return null;
        _inst = CoroutineSlave.Start(m_Job(this));
        return CurrentState;
    }
    public void Stop()
    {
        if (_inst != null) {
            CoroutineSlave.Stop(_inst);
            _inst = null;
        }
    }

    public AsyncState CurrentState {
        get {
            return m_State;
        }
    }
    public IEventListener EventListener {
        get {
            return m_Listener;
        }
    }
}
