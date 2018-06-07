using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EraseStickerButton : MonoBehaviour {
    
    public Rect buttonRect;
    public bool shouldOpen = false;
    public RectTransform coverRT;

    // === Core functions ==========================================================
    // ----------------------------------------------------------------
    void Awake () {
        buttonRect = StickerManager.GetWorldRectFromRT(this.GetComponent<RectTransform>());
    }

    // ----------------------------------------------------------------
    void Update () {
        float targetRotZ = (shouldOpen) ? 40 : 0;
        coverRT.localRotation = Quaternion.Euler(0, 0, targetRotZ);
    }

    // ----------------------------------------------------------------
    public void SetWasteBinOpen(bool open)
    {
        shouldOpen = open;
    }

    // ----------------------------------------------------------------
    public bool ContainsSticker(Vector3 position)
    {
        return buttonRect.Contains(position);
    }

    // ----------------------------------------------------------------
    public void Reset()
    {
        shouldOpen = false;
        coverRT.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
