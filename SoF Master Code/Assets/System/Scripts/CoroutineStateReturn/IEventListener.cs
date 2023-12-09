using System;
using System.Collections;

public interface IEventListener {
    void Register(string eventName, Delegate eventCallback);
    bool Contains(string eventName);
    T Call<T>(string eventName);
}

public class SimpleEventListener : IEventListener {
    Hashtable mTable = new Hashtable();

    public T Call<T>(string eventName)
    {
        return (T)mTable[eventName];
    }

    public bool Contains(string eventName)
    {
        return mTable.Contains(eventName);
    }

    public void Register(string eventName, Delegate eventCallback)
    {
        if (mTable.Contains(eventName)) {
            mTable[eventName] = Delegate.Combine((Delegate)mTable[eventName], eventCallback);
        } else {
            mTable.Add(eventName, eventCallback);
        }
    }
}

public delegate void ProgressUpdateCallback(float progress);
public delegate void PrintMessageCallback(string message);
public delegate void RequestFinishedCallback(bool isSuccess, string message);
public delegate void RequestedObjectCallback(object requestedObject);