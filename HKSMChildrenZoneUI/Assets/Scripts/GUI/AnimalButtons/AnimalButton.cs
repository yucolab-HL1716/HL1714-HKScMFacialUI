using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TouchScript;
using TouchScript.Gestures;

public class AnimalButton : MonoBehaviour {

    private PressGesture pressGesture;

    public int stickerID;
    public Rect buttonRect;

    // ----------------------------------------------------------------
    private void Awake()
    {
        buttonRect = AnimalStickerManager.GetWorldRectFromRT(this.GetComponent<RectTransform>());
    }

    // ----------------------------------------------------------------
    private void OnEnable()
    {
        pressGesture = GetComponent<PressGesture>();
        pressGesture.Pressed += PressHandler;
    }

    // ----------------------------------------------------------------
    private void OnDisable()
    {
        pressGesture.Pressed -= PressHandler;
    }

    // ----------------------------------------------------------------
    private void PressHandler(object sender, EventArgs eventArgs)
    {
        var stickerObj = AnimalStickerManager.instance.AddSticker(stickerID, GetComponent<RectTransform>().anchoredPosition);
        if (stickerObj == null)
            return;

        LayerManager.Instance.SetExclusive(stickerObj);
        pressGesture.Cancel(true, true);
        LayerManager.Instance.ClearExclusive();

    }

    // ----------------------------------------------------------------
    public void SetInteractable(bool interactable)
    {
        GetComponent<Button>().interactable = interactable;
    }
}
