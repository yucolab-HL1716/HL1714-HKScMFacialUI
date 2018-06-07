using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using DoozyUI;
using TMPro;
using DG.Tweening;

using Yucolab;

public class StillHerePopUpWindow : MonoBehaviour
{

    public TextMeshProUGUI countdownText;

    private int timeLimit = 10;
    private float remainingTime = 10;
    private float startTime = 0;
    public bool isVisible = false;

    public Canvas thisCanvas;


    // === Core functions =================================================================
    // ----------------------------------------------------------------
    void Start()
    {
        HideWindow();

        thisCanvas.overrideSorting = false;
        thisCanvas.overrideSorting = true;
        thisCanvas.sortingOrder++;
        thisCanvas.sortingOrder--;
    }

    // ----------------------------------------------------------------
    private void OnEnable()
    {
        thisCanvas.overrideSorting = false;
        thisCanvas.overrideSorting = true;
        thisCanvas.sortingOrder++;
        thisCanvas.sortingOrder--;
    }

    // ----------------------------------------------------------------
    void Update()
    {
        if (isVisible)
        {
            remainingTime = timeLimit - (Time.time - startTime);
            countdownText.SetText(Mathf.CeilToInt(remainingTime).ToString());
            if (remainingTime < 0)
            {
                HideWindow();
                GameRoot.instance.ResetGameOnTimeout();
            }
        }
    }

    // ----------------------------------------------------------------
    public void ShowWindow()
    {
        isVisible = true;
        remainingTime = timeLimit;
        startTime = Time.time;
        UIManager.ShowUiElement("PopUp_StillHere", "FacialGameMenu");


        thisCanvas.overrideSorting = false;
        thisCanvas.overrideSorting = true;
    }

    // ----------------------------------------------------------------
    public void HideWindow()
    {
        isVisible = false;
        remainingTime = timeLimit;
        startTime = 0;
        UIManager.HideUiElement("PopUp_StillHere", "FacialGameMenu");
    }

    // ----------------------------------------------------------------
    public void SetTimeLimit(int limit)
    {
        this.timeLimit = limit;
        this.remainingTime = limit;
        startTime = Time.time;
    }

    // ----------------------------------------------------------------
    public void OnStillHereButtonPressed()
    {
        GameRoot.instance.RestartScreensaverTimer();
        HideWindow();
    }
}
