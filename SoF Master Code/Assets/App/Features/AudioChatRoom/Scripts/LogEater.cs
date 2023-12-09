using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// used to consume unity's debug msgs & dump it onto unity ui to show on client device;
/// </summary>
[RequireComponent(typeof(UnityEngine.UI.Text))]
public class LogEater : MonoBehaviour {
    public int clearDelay = 30;
    Text destination;
    string temp;
	// Use this for initialization
	void OnEnable () {
        destination = GetComponent<Text>();
        destination.alignment = TextAnchor.LowerLeft;
        destination.supportRichText = true;
        destination.verticalOverflow = VerticalWrapMode.Overflow;
        Application.logMessageReceivedThreaded += logthreadedcallback;

        StartCoroutine(DelayClearLog());
    }
    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= logthreadedcallback;
        StopAllCoroutines();
    }


    IEnumerator DelayClearLog()
    {
        while (true)
        {
            yield return new WaitForSeconds(clearDelay);
            destination.text = "";
        }
    }
    void logthreadedcallback(string cond, string stacktrace, LogType logtype)
    {
        temp = destination.text;
        System.Text.StringBuilder build = new System.Text.StringBuilder();
        build.Append(destination.text);
        if (logtype == LogType.Log)
        {
            build.AppendLine(cond);
        }
        if (logtype == LogType.Error)
        {
            build.AppendLine("<color=#800000ff>" + cond+"</color>");
        }
        destination.text = build.ToString();
    }
}
