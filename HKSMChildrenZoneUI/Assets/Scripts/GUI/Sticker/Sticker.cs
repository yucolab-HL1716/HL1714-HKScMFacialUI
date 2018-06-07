using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TouchScript;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;

public class Sticker : MonoBehaviour
{
    public int stickerId = 0;

    public List<Sprite> spriteList;
    private Image img;

    private ScreenTransformGesture tranGesture;
    private PressGesture pressGesture;
    private ReleaseGesture releaseGesture;

    public bool isInsideEraseButton = false;

    public Vector3 releaseGesturePosition;

    // ----------------------------------------------------------------
    private void OnEnable()
    {
        // Setup Touchscript gestures
        tranGesture = GetComponent<ScreenTransformGesture>();
        tranGesture.TransformStarted += TransformStartHandler;
        tranGesture.Transformed += TransformOngoingHandler;

        releaseGesture = GetComponent<ReleaseGesture>();
        releaseGesture.Released += ReleaseHandler;

        pressGesture = GetComponent<PressGesture>();
        pressGesture.Pressed += PressHandler;
    }

    // ----------------------------------------------------------------
    private void OnDisable()
    {
        // Remove TouchScript gesture handlers
        tranGesture.TransformStarted -= TransformStartHandler;
        tranGesture.Transformed -= TransformOngoingHandler;
        pressGesture.Pressed -= PressHandler;
        releaseGesture.Released -= ReleaseHandler;
    }
    
    // ----------------------------------------------------------------
    public void Setup(int _id, Vector2 _pos)
    {
        stickerId = _id;
        img = GetComponent<Image>();
        img.sprite = spriteList[stickerId];
        img.rectTransform.sizeDelta = img.sprite.rect.size;
        img.rectTransform.anchoredPosition = _pos;
        this.name = "DecorationSticker_" + stickerId.ToString();
    }

    // ----------------------------------------------------------------
    private void TransformStartHandler(object sender, EventArgs eventArgs)
    {
        this.transform.SetAsLastSibling();
    }

    // ----------------------------------------------------------------
    private void TransformOngoingHandler(object sender, EventArgs eventArgs)
    {
        // Check if it is inside erase button
        Vector2 transformPosition = tranGesture.ScreenPosition;
        isInsideEraseButton = StickerManager.instance.eraseButton.ContainsSticker(transformPosition);
    }


    // ----------------------------------------------------------------
    private void PressHandler(object sender, EventArgs eventArgs)
    {
        this.transform.SetAsLastSibling();
    }

    // ----------------------------------------------------------------
    private void ReleaseHandler(object sender, EventArgs eventArgs)
    {
        releaseGesturePosition = releaseGesture.ScreenPosition;
        bool shouldErase = StickerManager.instance.eraseButton.ContainsSticker(releaseGesturePosition);

        // Remove sticker if it is outside the rectangular area
        if (!rectOverlaps(this.GetComponent<RectTransform>(), StickerManager.instance.maskRect) || shouldErase)
        {
            StickerManager.instance.RemoveSticker(this);
        }
        else
        {
            this.transform.SetParent(StickerManager.instance.maskStickerHolder);
            this.transform.SetAsLastSibling();
        }        
    }

    // ----------------------------------------------------------------
    private bool rectOverlaps(RectTransform rectTrans1, Rect rect2)
    {
        Rect rect1 = StickerManager.GetWorldRectFromRT(rectTrans1);
        return rect1.Overlaps(rect2);
    }
}
