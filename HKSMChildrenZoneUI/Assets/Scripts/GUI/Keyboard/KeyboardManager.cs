using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DoozyUI;
using TMPro;
using DG.Tweening;

public class KeyboardManager : MonoSingleton<KeyboardManager>
{
    public TextMeshProUGUI field_email_p1;

    public Image inputAreaP1;
    public string inputEmailP1;

    public TextMeshProUGUI tmp_emailList;

    public Image invalidWarning;
    private Sequence seq_invalidEmail;

    // ----------------------------------------------------------------
    public override void Init()
    {
        
    }

    // ----------------------------------------------------------------
    void OnEnable()
    {
        field_email_p1.raycastTarget = false;
    }

    // ----------------------------------------------------------------
    public void ResetKeyboard()
    {
        inputEmailP1 = "";
        field_email_p1.text = "";
        tmp_emailList.text = "";
        invalidWarning.DOFade(0, 0);
    }
    
    // ----------------------------------------------------------------
    public void OnKeyUIButtonPressed (string _key) {
		switch(_key)
        {
            case "backspace":
                if (inputEmailP1.Length > 0) {
                    inputEmailP1 = inputEmailP1.Substring(0, inputEmailP1.Length - 1);
                    field_email_p1.text = inputEmailP1;
                }
                break;
            case "Send":
                // Validate email
                if(IsValidEmail(inputEmailP1))
                {
                    YucoDebugger.instance.Log("Input Email P1 = " + inputEmailP1 + ", email OK", "OnKeyUIButtonPressed", "KeyboardManager");
                    // Change to confirm page
                    tmp_emailList.text = inputEmailP1;
                    UIManager.ShowUiElement("PopUp_ConfirmEmail", "FacialGameMenu");
                    invalidWarning.DOFade(0, 0);
                } else {
                    YucoDebugger.instance.Log("Input Email P1 = " + inputEmailP1 + ", email invalid!", "OnKeyUIButtonPressed", "KeyboardManager");

                    seq_invalidEmail.Kill(false);
                    seq_invalidEmail = DOTween.Sequence();
                    seq_invalidEmail.Append(invalidWarning.DOFade(1, 0.3f));
                    seq_invalidEmail.AppendInterval(1f);
                    seq_invalidEmail.Append(invalidWarning.DOFade(0, 2f));
                }
                break;
            default:
                if (inputEmailP1.Length < 40)
                {
                    inputEmailP1 += _key;
                    field_email_p1.text = inputEmailP1;
                }
                break;
        }
	}

    // ----------------------------------------------------------------
    public void OnConfirmEmailClicked()
    {
        GameRoot.instance.EmailToVisitors(inputEmailP1);
    }


    // ----------------------------------------------------------------
    private bool IsValidEmail(string email)
    {
        try
        {
            var mail = new System.Net.Mail.MailAddress(email);
            return mail.Host.Contains(".");
        }
        catch
        {
            return false;
        }
    }
}
