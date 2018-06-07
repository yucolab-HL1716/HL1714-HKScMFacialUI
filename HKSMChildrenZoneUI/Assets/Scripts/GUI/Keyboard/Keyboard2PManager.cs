using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DoozyUI;
using TMPro;
using DG.Tweening;

public class Keyboard2PManager : MonoSingleton<Keyboard2PManager>
{
    public GameObject label_p1, label_p2;

    public TextMeshProUGUI field_email_p1;
    public TextMeshProUGUI field_email_p2;

    public Image inputAreaP1, inputAreaP2;
    public string inputEmailP1, inputEmailP2;

    public TextMeshProUGUI tmp_emailList;

    public Image invalidWarning;
    private Sequence seq_invalidEmail;

    public bool bEditP1 = true;

    // ----------------------------------------------------------------
    public override void Init()
    {
        
    }

    // ----------------------------------------------------------------
    void OnEnable()
    {
        field_email_p1.raycastTarget = false;
        field_email_p2.raycastTarget = false;
    }

    // ----------------------------------------------------------------
    public void ResetKeyboard()
    {
        inputEmailP1 = inputEmailP2 = "";
        field_email_p1.text = "";
        field_email_p2.text = "";
        tmp_emailList.text = "";

        invalidWarning.DOFade(0, 0);
        OnInputFieldSelected(0);
    }

    // ----------------------------------------------------------------
    public void OnInputFieldSelected(int _id)
    {
        bEditP1 = (_id == 0);

        if (bEditP1)
        {
            inputAreaP1.color = new Color(1, 218.0f / 255.0f, 163.0f / 255.0f);
            inputAreaP2.color = Color.white;
        }
        else
        {
            inputAreaP1.color = Color.white;
            inputAreaP2.color = new Color(1, 218.0f / 255.0f, 163.0f / 255.0f);
        }
    }


    // ----------------------------------------------------------------
    public void OnKeyUIButtonPressed(string _key)
    {
        switch (_key)
        {
            case "backspace":
                if (bEditP1)
                {
                    if (inputEmailP1.Length > 0)
                    {
                        inputEmailP1 = inputEmailP1.Substring(0, inputEmailP1.Length - 1);
                        field_email_p1.text = inputEmailP1;
                    }
                }
                else
                {
                    if (inputEmailP2.Length > 0)
                    {
                        inputEmailP2 = inputEmailP2.Substring(0, inputEmailP2.Length - 1);
                        field_email_p2.text = inputEmailP2;
                    }
                }
                break;
            case "Send":

                bool validEmailP1 = IsValidEmail(inputEmailP1);
                bool emptyEmailP1 = inputEmailP1 == "";
                bool validEmailP2 = IsValidEmail(inputEmailP2);
                bool emptyEmailP2 = inputEmailP2 == "";

                bool shouldSendEmail = (validEmailP1 && validEmailP2) || (validEmailP1 && emptyEmailP2) || (validEmailP2 && emptyEmailP1);
                
                if (validEmailP1) {
                    YucoDebugger.instance.Log("Input Email P1 = " + inputEmailP1 + ", email OK", "OnKeyUIButtonPressed", "KeyboardManager");
                } else {
                    YucoDebugger.instance.Log("Input Email P1 = " + inputEmailP1 + ", email invalid!", "OnKeyUIButtonPressed", "KeyboardManager");
                }

                if (validEmailP2) {
                    YucoDebugger.instance.Log("Input Email P2 = " + inputEmailP2 + ", email OK", "OnKeyUIButtonPressed", "KeyboardManager");
                } else {
                    YucoDebugger.instance.Log("Input Email P2 = " + inputEmailP2 + ", email invalid!", "OnKeyUIButtonPressed", "KeyboardManager");
                }
                                
                if (shouldSendEmail)
                {
                    if (validEmailP1 && validEmailP2)
                    {
                        tmp_emailList.text = inputEmailP1 + "\n" + inputEmailP2;
                    }
                    else if(emptyEmailP1)
                    {
                        tmp_emailList.text = inputEmailP2;
                    }
                    else if (emptyEmailP2)
                    {
                        tmp_emailList.text = inputEmailP1;
                    }

                    // Change to confirm page
                    UIManager.ShowUiElement("PopUp_ConfirmEmail", "FacialGameMenu");
                    invalidWarning.DOFade(0, 0);
                }
                else
                {
                    seq_invalidEmail.Kill(false);
                    seq_invalidEmail = DOTween.Sequence();
                    seq_invalidEmail.Append(invalidWarning.DOFade(1, 0.3f));
                    seq_invalidEmail.AppendInterval(1f);
                    seq_invalidEmail.Append(invalidWarning.DOFade(0, 2f));
                }

                break;
            default:
                if (bEditP1)
                {
                    if (inputEmailP1.Length < 40)
                    {
                        inputEmailP1 += _key;
                        field_email_p1.text = inputEmailP1;
                    }
                }
                else
                {
                    if (inputEmailP2.Length < 40)
                    {
                        inputEmailP2 += _key;
                        field_email_p2.text = inputEmailP2;
                    }
                }
                break;
        }
    }

    // ----------------------------------------------------------------
    public void OnConfirmEmailClicked()
    {   
        if (inputEmailP1 == "")
        {
            GameRoot.instance.EmailToVisitors(inputEmailP2);
        }
        else if (inputEmailP2 == "")
        {
            GameRoot.instance.EmailToVisitors(inputEmailP1);
        }
        else
        {
            GameRoot.instance.EmailToVisitors(inputEmailP1, inputEmailP2);
        }
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
