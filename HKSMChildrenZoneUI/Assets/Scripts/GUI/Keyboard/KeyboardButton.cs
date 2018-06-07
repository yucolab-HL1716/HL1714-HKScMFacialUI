using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DoozyUI;
using TMPro;

public class KeyboardButton : MonoBehaviour
{

    public bool assignSuffixToLabel = true;
    public string key;

    private UIButton uiBtn;
    private TextMeshProUGUI TMPText;

    // ----------------------------------------------------------------
    void Awake()
    {        
        uiBtn = GetComponent<UIButton>();

        key = this.name.Remove(0, 13);
        if (assignSuffixToLabel)
        {
            TMPText = GetComponentInChildren<TextMeshProUGUI>();
            TMPText.text = key;
        }

        if (key != "backspace")
        {
            uiBtn.OnClick.AddListener(onKeyUIBtnClicked);
        }
        else
        {
            uiBtn.OnClick.AddListener(onKeyUIBtnClicked);
        }
    }

    // ----------------------------------------------------------------
    public void onKeyUIBtnClicked()
    {
        if(transform.parent.GetComponent<KeyboardManager>() != null)
            KeyboardManager.instance.OnKeyUIButtonPressed(key);
        if (transform.parent.GetComponent<Keyboard2PManager>() != null)
            Keyboard2PManager.instance.OnKeyUIButtonPressed(key);
    }
}
