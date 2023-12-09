using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericProgressGUI : MonoBehaviour {

    [SerializeField]
    Slider m_ProgressBar;
    [SerializeField]
    Text m_TextDisplay;
    [SerializeField]
    Text m_PercentageDisplay;
    [SerializeField]
    CanvasGroup m_CanvasGroup;

    public void Reset()
    {
        m_ProgressBar.value = 0;
        m_ProgressBar.minValue = 0;
        m_ProgressBar.maxValue = 1;

        ClearInfo();
        ClearProgress();
    }
    public void SetProgress(float percentage)
    {
        m_ProgressBar.value = percentage;
        m_PercentageDisplay.text = percentage.ToString("p2");
    }
    public void ClearProgress()
    {
        SetProgress(0);
        m_PercentageDisplay.text = "";
    }
    public void ShowInfo(string message)
    {
        m_TextDisplay.text = message;
    }
    public void ClearInfo()
    {
        ShowInfo("");
    }

    public void Show()
    {
        m_CanvasGroup.alpha = 1;
    }
    public void Hide()
    {
        m_CanvasGroup.alpha = 0;
    }
}
