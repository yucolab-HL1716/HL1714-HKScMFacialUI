using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TouchScript;
using TouchScript.Gestures;

public class StickerButton : MonoBehaviour {

    private PressGesture pressGesture;

    public int stickerID;

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
        var stickerObj = StickerManager.instance.AddSticker(stickerID, GetComponent<RectTransform>().anchoredPosition);
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
