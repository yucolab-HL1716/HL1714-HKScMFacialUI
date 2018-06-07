using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class UIFlowEffect : MonoBehaviour
{
    public float delay;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(StartSequence());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(delay);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(GetComponent<RectTransform>().DOJumpAnchorPos(new Vector2(GetComponent<RectTransform>().anchoredPosition.x, GetComponent<RectTransform>().anchoredPosition.y), 20, 1, 2f));
        mySequence.SetLoops(-1, LoopType.Restart);
    }
}
