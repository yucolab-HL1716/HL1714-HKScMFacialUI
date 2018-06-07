using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ImageLanguageChangeReceiver : MonoBehaviour {

    public bool isDynamicSize = true;
    public Sprite englishSprite;
    public Sprite chineseSprite;


    private bool bAppQuited = false;

    // === Core functions ========================================================================================================================================
    // ---------------------------------------------------------------
    void OnEnable()
    {
        LanguageManager.instance.LanguageChanged += OnLanguageChanged;
        
        if (LanguageManager.instance.IsEnglish())
        {
            SetSprite(englishSprite);
        }
        else
        {
            SetSprite(chineseSprite);
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
    private void SetSprite(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;

        if(isDynamicSize)
            GetComponent<RectTransform>().sizeDelta = sprite.rect.size;
    }
    
    // ---------------------------------------------------------------
    private void OnLanguageChanged(object sender, LanguageChangeEventArgs e)
    {
        if(e.currentLanguage == Language.English)
        {
            SetSprite(englishSprite);
        }
        else
        {
            SetSprite(chineseSprite);
        }
    }
}
