using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBar : MonoBehaviour {

    public GameMode gameMode;
    public List<Sprite> chineseSprites;
    public List<Sprite> englishSprites;
    
    private bool bAppQuited = false;
    private int currentId = 0;

    // ---------------------------------------------------------------
    void OnEnable()
    {
        LanguageManager.instance.LanguageChanged += OnLanguageChanged;

        SetSprite(LanguageManager.instance.currentLanguage, 0);
    }

    // ---------------------------------------------------------------
    void OnDisable()
    {
        if (!bAppQuited)
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
        SetSprite(e.currentLanguage, currentId);
    }

    // ---------------------------------------------------------------
    private void SetSprite(Language language, int id)
    {
        if (language == Language.English)
        {
            if (id > englishSprites.Count - 1)
                return;

            currentId = id;
            GetComponent<Image>().sprite = englishSprites[id];
            GetComponent<RectTransform>().sizeDelta = englishSprites[id].rect.size;
        }
        else
        {
            if (id > chineseSprites.Count - 1)
                return;

            currentId = id;
            GetComponent<Image>().sprite = chineseSprites[id];
            GetComponent<RectTransform>().sizeDelta = chineseSprites[id].rect.size;
        }
    }

    // ----------------------------------------------------------------
    public void Reset()
    {
        SetSprite(LanguageManager.instance.currentLanguage, 0);
    }

    // ----------------------------------------------------------------
    public void SetIcon (int id) {
        SetSprite(LanguageManager.instance.currentLanguage, id);
    }
}
