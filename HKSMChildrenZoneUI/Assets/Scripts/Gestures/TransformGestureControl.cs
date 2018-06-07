using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;

public class TransformGestureControl : MonoBehaviour
{
    private ScreenTransformGesture gesture;
    public ScreenTransformGesture neighbourGesture;
    public Gesture.GestureState state;
    public Gesture.GestureState neighbourState;
    public RectTransform neighbourRT;

    private RectTransform dragImg;
    private RectTransform maskImg;

    private Vector2 dragPos, dragSize, maskPos, maskSize;
    private Vector2 maskTL, maskBR, dragTL, dragBR;

    private Vector2 startPos;

    public float scaleBuffer = 0.02f;
    public float distanceBuffer = 5f;

    public bool isUserLearnt = false;
    public List<GameObject> gestureHints;

    // ----------------------------------------------------------------
    // Use this for initialization
    void Awake()
    {
        isUserLearnt = false;
        dragImg = GetComponent<RectTransform>();
        if (maskImg == null)
            maskImg = transform.parent.GetComponent<RectTransform>();
        gesture = GetComponent<ScreenTransformGesture>();
        
        maskPos = maskImg.position;
        maskSize = maskImg.sizeDelta;
        maskTL = new Vector2(maskPos.x - maskSize.x / 2, maskPos.y + maskSize.y / 2);
        maskBR = new Vector2(maskPos.x + maskSize.x / 2, maskPos.y - maskSize.y / 2);

        startPos = dragPos = dragImg.position;
        Vector2 dragScale = dragImg.localScale;
        dragSize = new Vector2(dragImg.sizeDelta.x * dragScale.x, dragImg.sizeDelta.y * dragScale.y);

        dragTL = new Vector2(dragPos.x - dragSize.x / 2, dragPos.y + dragSize.y / 2);
        dragBR = new Vector2(dragPos.x + dragSize.x / 2, dragPos.y - dragSize.y / 2);
    }
    
    // ----------------------------------------------------------------
    public void ResetState()
    {
        isUserLearnt = false;
        SetGestureHintsEnable(true);
        GetComponent<RectTransform>().localScale = Vector3.one;
        GetComponent<RectTransform>().position = startPos;
    }

    // ----------------------------------------------------------------
    // Update is called once per frame
    void LateUpdate()
    {
        if (this.enabled && !this.isActiveAndEnabled)
        {
            ResetState();
            return;
        }

        state = gesture.State;
        if(neighbourGesture != null)
            neighbourState = neighbourGesture.State;


        if (state == Gesture.GestureState.Changed)
        {
            isUserLearnt = true;
            gestureHints.ForEach(go => go.SetActive(false && go.activeSelf));
            RectSizeCheck();
        }
    }

    // ----------------------------------------------------------------
    public void OnTransformGestureStart()
    {
        if(neighbourState != Gesture.GestureState.Idle)
            gesture.Cancel();
    }

    // ----------------------------------------------------------------
    public void OnTransformGestureOngoing()
    {
        if (gesture == null)
            return;
    }

    // ----------------------------------------------------------------
    public void OnTransformGestureComplete()
    {
        RectSizeCheck();
    }

    // ----------------------------------------------------------------
    public void SetGestureHintsEnable(bool enabled)
    {
        gestureHints.ForEach(go => go.SetActive(enabled));
    }

    // ----------------------------------------------------------------
    public void RectSizeCheck()
    {
        dragPos = dragImg.position;
        Vector2 dragScale = dragImg.localScale;
        dragSize = new Vector2(dragImg.sizeDelta.x * dragScale.x, dragImg.sizeDelta.y * dragScale.y);

        dragTL = new Vector2(dragPos.x - dragSize.x / 2, dragPos.y + dragSize.y / 2);
        dragBR = new Vector2(dragPos.x + dragSize.x / 2, dragPos.y - dragSize.y / 2);

        float minScale = SettingManager.instance.dragMinimumScale;
        if (dragImg.localScale.x < minScale
            || dragImg.localScale.y < minScale
            || dragImg.localScale.z < minScale)
        {
            dragImg.localScale = new Vector3(minScale, minScale, minScale);
        }
        if (dragSize.y < maskSize.y)
        {
            float targetScale = maskSize.y / dragImg.sizeDelta.y + scaleBuffer;
            dragImg.localScale = new Vector3(targetScale, targetScale, targetScale);
        }
        else if (dragSize.x < maskSize.x)
        {
            float targetScale = maskSize.x / dragImg.sizeDelta.x + scaleBuffer;
            dragImg.localScale = new Vector3(targetScale, targetScale, targetScale);
        }

        if (dragTL.x > maskTL.x) // left
        {
            dragImg.position = new Vector2(dragImg.position.x - (dragTL.x - maskTL.x) - distanceBuffer, dragImg.position.y);
        }
        if (dragTL.y < maskTL.y) // top
        {
            dragImg.position = new Vector2(dragImg.position.x, dragImg.position.y + (maskTL.y - dragTL.y) + distanceBuffer);
        }
        if (dragBR.x < maskBR.x) // right
        {
            dragImg.position = new Vector2(dragImg.position.x + (maskBR.x - dragBR.x) + distanceBuffer, dragImg.position.y);
        }
        if (dragBR.y > maskBR.y) // bottom
        {
            dragImg.position = new Vector2(dragImg.position.x, dragImg.position.y - (dragBR.y - maskBR.y) - distanceBuffer);
        }

        if (neighbourRT != null)
        {
            neighbourRT.localScale = dragImg.localScale;
            neighbourRT.position = new Vector2(startPos.x - (dragImg.position.x - startPos.x), dragImg.position.y);
        }
    }
}
