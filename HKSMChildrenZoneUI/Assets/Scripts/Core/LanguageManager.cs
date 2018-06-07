using UnityEngine;
using System;
using System.Collections.Generic;


public enum Language
{
    Chinese,
    English,
}

public class LanguageChangeEventArgs : EventArgs
{
    public Language currentLanguage;

    // ---------------------------------------------------------------
    public LanguageChangeEventArgs(Language language)
    {
        currentLanguage = language;
    }
}


public class LanguageManager : MonoSingleton<LanguageManager> 
{
    public delegate void LanguageChangeEventDlgt(object sender, LanguageChangeEventArgs e);
    public event LanguageChangeEventDlgt LanguageChanged = delegate { };

    public Language currentLanguage;

    // ---------------------------------------------------------------
    public override void Init()
    {
        currentLanguage = Language.Chinese;
    }

    // ---------------------------------------------------------------
    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        LanguageChanged(this, new LanguageChangeEventArgs(currentLanguage));
    }

    // ---------------------------------------------------------------
    public void ToggleLanguage()
    {
        currentLanguage = (currentLanguage == Language.English) ? Language.Chinese : Language.English;
        LanguageChanged(this, new LanguageChangeEventArgs(currentLanguage));
    }

    // ---------------------------------------------------------------
    public bool IsEnglish()
    {
        return (currentLanguage == Language.English);
    }
}