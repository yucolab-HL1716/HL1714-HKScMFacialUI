using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFullscreen : MonoBehaviour {

    public float lastFullscreenTime = 0;
    public float fullscreenInterval = 60;
    public int targetWidth = 1920;
    public int targetHeight = 1080;

    // Use this for initialization
    void Start () {
        lastFullscreenTime = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if(Time.time - lastFullscreenTime > fullscreenInterval)
        {
            lastFullscreenTime = Time.time;
            Screen.SetResolution(targetWidth, targetHeight, true);
        }
	}
}
