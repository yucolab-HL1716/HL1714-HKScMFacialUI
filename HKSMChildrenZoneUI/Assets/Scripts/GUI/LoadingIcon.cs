using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;


public class LoadingIcon : MonoBehaviour {

    public float speed = 1;

	// === Core functions =================================================================
	// ----------------------------------------------------------------
    private void Update()
    {
        GetComponent<RectTransform>().Rotate(new Vector3(0, 0, Time.deltaTime * speed));
    }
    
}
