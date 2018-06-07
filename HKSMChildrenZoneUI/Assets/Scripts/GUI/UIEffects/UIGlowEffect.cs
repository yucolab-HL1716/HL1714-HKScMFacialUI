using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class UIGlowEffect : MonoBehaviour {

    public Color originalColor = Color.white;
    public Color highlightColor = new Color(0.75f, 0.75f, 0.75f);

	// Use this for initialization
	void Start () {
		
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(GetComponent<Image>().DOColor(highlightColor, 1f));
        mySequence.Append(GetComponent<Image>().DOColor(originalColor, 1f));
        mySequence.SetLoops(-1, LoopType.Restart);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
