using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TouchScript;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;

public class AnimalSticker : MonoBehaviour
{
    public int stickerId = 0;

    public List<Sprite> spriteList;
    private Image img;
    
    private ReleaseGesture releaseGesture;

    public Vector3 releaseGesturePosition;

    // ----------------------------------------------------------------
    private void OnEnable()
    {
        // Setup Touchscript gestures
        releaseGesture = GetComponent<ReleaseGesture>();
        releaseGesture.Released += ReleaseHandler;
    }

    // ----------------------------------------------------------------
    private void OnDisable()
    {
        // Remove TouchScript gesture handlers
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
        this.name = "AnimalSticker_" + stickerId.ToString();
    }
    

    // ----------------------------------------------------------------
    private void ReleaseHandler(object sender, EventArgs eventArgs)
    {
        releaseGesturePosition = releaseGesture.ScreenPosition;

        // Trigger animal selection if it is inside animal button or inside image mask area
        if (rectOverlaps(this.GetComponent<RectTransform>(), AnimalStickerManager.instance.animalButtons[stickerId].buttonRect))
        {
            Game3Manager.instance.OnAnimalButtonClicked(stickerId);
            AnimalStickerManager.instance.RemoveSticker(this);
        }
        else if (rectOverlaps(this.GetComponent<RectTransform>(), AnimalStickerManager.instance.maskRect))
        {
            Game3Manager.instance.OnAnimalButtonClicked(stickerId);
            AnimalStickerManager.instance.RemoveSticker(this);
        }
        else
        // Remove animal sticker
        {           
            AnimalStickerManager.instance.RemoveSticker(this);
        }        
    }

    // ----------------------------------------------------------------
    private bool rectOverlaps(RectTransform rectTrans1, Rect rect2)
    {
        Rect rect1 = StickerManager.GetWorldRectFromRT(rectTrans1);
        return rect1.Overlaps(rect2);
    }
}
