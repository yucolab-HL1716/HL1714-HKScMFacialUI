using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundColorByGame : MonoBehaviour {

    public List<Color> backgroundColors;

	// Use this for initialization
	void Start () {
		
	}
	
	public void UpdateBackgroundColor(GameMode gameMode)
    {
        switch(gameMode)
        {
            case GameMode.Nil:
                this.GetComponent<Image>().color = backgroundColors[0];
                break;
            case GameMode.GAME1:
                this.GetComponent<Image>().color = backgroundColors[1];
                break;
            case GameMode.GAME2:
                this.GetComponent<Image>().color = backgroundColors[2];
                break;
            case GameMode.GAME3:
                this.GetComponent<Image>().color = backgroundColors[3];
                break;
        }
    }
}
