using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Yucolab;

[RequireComponent(typeof(Image))]
public class ImageLanguageChangeWithGame : MonoBehaviour
{
    public bool isDynamicSize = true;

    public Sprite englishSprite_game1;
    public Sprite chineseSprite_game1;
    public Sprite englishSprite_game2;
    public Sprite chineseSprite_game2;
    public Sprite englishSprite_game3;
    public Sprite chineseSprite_game3;
    
    private bool bAppQuited = false;

    // === Core functions ========================================================================================================================================
    // ---------------------------------------------------------------
    void OnEnable()
    {
        LanguageManager.instance.LanguageChanged += OnLanguageChanged;
        GameRoot.instance.GameModeChanged += OnGameModeChanged;

        SetSprite(LanguageManager.instance.currentLanguage);
    }

    // ---------------------------------------------------------------
    void OnDisable()
    {
        if (!bAppQuited)
        {
            LanguageManager.instance.LanguageChanged -= OnLanguageChanged;
            GameRoot.instance.GameModeChanged -= OnGameModeChanged;
        }
    }

    // ---------------------------------------------------------------
    void OnApplicationQuit()
    {
        bAppQuited = true;
    }

    // ---------------------------------------------------------------
    private void SetSprite(Language language)
    {
        if(GameRoot.instance.gameMode == GameMode.GAME1)
        {
            if(language == Language.English)
            {
                SetSprite(englishSprite_game1);
            }
            else
            {
                SetSprite(chineseSprite_game1);
            }
        }
        else if (GameRoot.instance.gameMode == GameMode.GAME2)
        {
            if (language == Language.English)
            {
                SetSprite(englishSprite_game2);
            }
            else
            {
                SetSprite(chineseSprite_game2);
            }
        }
        else if (GameRoot.instance.gameMode == GameMode.GAME3)
        {
            if (language == Language.English)
            {
                SetSprite(englishSprite_game3);
            }
            else
            {
                SetSprite(chineseSprite_game3);
            }
        }
    }

    // ---------------------------------------------------------------
    private void SetSprite(Sprite sprite)
    {
        GetComponent<Image>().sprite = sprite;

        if (isDynamicSize)
            GetComponent<RectTransform>().sizeDelta = sprite.rect.size;
    }

    // ---------------------------------------------------------------
    private void OnLanguageChanged(object sender, LanguageChangeEventArgs e)
    {
        SetSprite(e.currentLanguage);
    }

    // ---------------------------------------------------------------
    private void OnGameModeChanged(object sender, GameModeChangeEventArgs e)
    {
        SetSprite(LanguageManager.instance.currentLanguage);
    }
}
