using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChompChompGameMenuController : MonoBehaviour {

    public CanvasGroup mainPanelCanvasGroup;
    public GameObject pausePanel;
    public Stage thisStage;
    public bool isPaused;
    
    // Use this for initialization
    void Start()
    {
        pausePanel.SetActive(false);
        mainPanelCanvasGroup.gameObject.SetActive(true);
        isPaused = false;
    }

    void Hide()
    {
        mainPanelCanvasGroup.alpha = 0;
        mainPanelCanvasGroup.blocksRaycasts = false;
        mainPanelCanvasGroup.interactable = false;
    }

    public void ReloadButtonClick()
    {
        Time.timeScale = 1;
        Hide();
        MainNavigationController.DoAssetBundleLoadLevel(Constants.CHOMPCHOMP_SCENES, "chompReloadProxy");
    }

    public void PauseButtonClick()
    {
        //Cannot use timescale = 0 because pause panel animation requires timescale to animate
        isPaused = true;
        pausePanel.SetActive(true);
    }
    public void PauseAnimationClick()
    {
        // Use timeScale = 0 to stop animations
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeButtonClick()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }
    public void ResumeAnimationClick()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void ExitButtonClick()
    {
        Hide();
        MainNavigationController.GotoMainMenu();

        if (Time.timeScale < 1f) {
            Time.timeScale = 1f;
        }
    }
    void PlayStarSFX()
    {
        AudioSystem.PlaySFX("ui/sfx_ui_get_star");
    }
}
