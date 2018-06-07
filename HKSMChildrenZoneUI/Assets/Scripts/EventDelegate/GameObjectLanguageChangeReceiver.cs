using UnityEngine;
using System.Collections;

public class GameObjectLanguageChangeReceiver : MonoBehaviour {
    
    public GameObject[] chineseObjects;
    public GameObject[] englishObjects;

    private bool bAppQuited = false;

    // === Core functions ========================================================================================================================================
    // ---------------------------------------------------------------
    void OnEnable()
    {
        LanguageManager.instance.LanguageChanged += OnLanguageChanged;

        foreach (GameObject go in chineseObjects)
        {
            go.SetActive(!LanguageManager.instance.IsEnglish());
        }
        foreach (GameObject go in englishObjects)
        {
            go.SetActive(LanguageManager.instance.IsEnglish());
        }
    }

    // ---------------------------------------------------------------
    void OnDisable()
    {
        if(!bAppQuited)
            LanguageManager.instance.LanguageChanged -= OnLanguageChanged;
    }

    // ---------------------------------------------------------------
    void OnApplicationQuit()
    {
        bAppQuited = true;
    }

    // ---------------------------------------------------------------
    private void OnLanguageChanged(object sender, LanguageChangeEventArgs e)
    {
        foreach (GameObject go in chineseObjects)
        {
            go.SetActive(e.currentLanguage == Language.Chinese);
        }
        foreach (GameObject go in englishObjects)
        {
            go.SetActive(e.currentLanguage == Language.English);
        }
    }
}
