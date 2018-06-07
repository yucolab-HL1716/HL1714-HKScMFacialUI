using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using RESTClient;
using LitJson;

public class FacialEntry
{
    public string   computer_id { get; set; }
    public int      singlePlay { get; set; }
    public int      groupPlay { get; set; }
    public int      animal { get; set; }
    public int      lang_chi { get; set; }
    public int      lang_eng { get; set; }
}

public class VisitEntry
{
    public int      lang_tchi { get; set; }
    public int      lang_eng { get; set; }
}

public class WebApiManager : MonoSingleton<WebApiManager> {
    
    private enum API_URL {
        CREATE,
        SELECT_QR,
        QR_UPLOADED,
        SELECT_EMAIL,
        SEND_EMAIL,
    }

    private enum VISIT_API_URL
    {
        CREATE,
        SET_TIMEOUT,
    }

    private SettingManager settingMgr;

    private string playerId = "";
    private string visitPlayerId = "";
    private FacialEntry facialEntry;
    private VisitEntry visitEntry;

    // === Core functions =================================================================
    // ---------------------------------------------------------------
    public override void Init() {
        settingMgr = SettingManager.instance;
    }
    
    // ----------------------------------------------------------------
    public void Reset()
    {
        playerId = "";
        facialEntry = new FacialEntry();
        facialEntry.computer_id = settingMgr.computerId;
        facialEntry.singlePlay = 0;
        facialEntry.groupPlay = 0;
        facialEntry.animal = 0;
        facialEntry.lang_chi = 0;
        facialEntry.lang_eng = 0;

        visitPlayerId = "";
        visitEntry = new VisitEntry();
        visitEntry.lang_eng = 0;
        visitEntry.lang_tchi = 0;
    }

    // ======================================================================================================================================================================
    //   Facial API functions 
    // ======================================================================================================================================================================

    // === CREATE functions =================================================================
    // ----------------------------------------------------------------
    public void RecordNewGameplay(int gameMode, Language language)
    {
        facialEntry.singlePlay = (gameMode == 0) ? 1 : 0;
        facialEntry.groupPlay = (gameMode == 1) ? 1 : 0;
        facialEntry.animal = (gameMode == 2) ? 1 : 0;
        facialEntry.lang_chi = (language == Language.Chinese) ? 1 : 0;
        facialEntry.lang_eng = (language == Language.English) ? 1 : 0;

        StartCoroutine(CreatePost(CreatePostCompleted));
    }

    // ----------------------------------------------------------------
    private IEnumerator CreatePost(Action<IRestResponse<string>> callback = null)
    {
        RestRequest request = new RestRequest(GetApiUrl(API_URL.CREATE), Method.POST);

        string jsonString = JsonMapper.ToJson(facialEntry);

        request.AddHeader("Content-Type", "application/json");
        request.AddBody(jsonString);

        YucoDebugger.instance.Log("Facial API: create facial entry: " + GetApiUrl(API_URL.CREATE), "CreatePost", "WebApiManager");

        yield return request.Request.SendWebRequest();
        request.GetText(callback);
    }

    // ----------------------------------------------------------------
    private void CreatePostCompleted(IRestResponse<string> response)
    {
        if (response.IsError)
        {
            YucoDebugger.instance.LogError("Facial API: create facial entry Error, StatusCode = " + response.StatusCode, "CreatePostCompleted", "WebApiManager");
            YucoDebugger.instance.LogError("Facial API: Error detail = " + response.ErrorMessage, "CreatePostCompleted", "WebApiManager");
            return;
        }
        YucoDebugger.instance.Log("Facial API:  create facial entry succeeded. id = " + response.Content);
        playerId = response.Content;
    }

    // === SELECT QR functions =================================================================
    // ----------------------------------------------------------------
    public void RecordSelectQR()
    {
        StartCoroutine(SelectQrPatch(SelectQrPatchCompleted));
    }

    // ----------------------------------------------------------------
    private IEnumerator SelectQrPatch(Action<IRestResponse<string>> callback = null)
    {
        RestRequest request = new RestRequest(GetApiUrl(API_URL.SELECT_QR), Method.PATCH);        
        YucoDebugger.instance.Log("Facial API: Select QR for id = " + playerId + ": " + GetApiUrl(API_URL.SELECT_QR), "SelectQrPatch", "WebApiManager");

        yield return request.Request.SendWebRequest();
        request.GetText(callback);
    }

    // ----------------------------------------------------------------
    private void SelectQrPatchCompleted(IRestResponse<string> response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            YucoDebugger.instance.LogError("Facial API: Select QR for id = " + playerId + ", StatusCode = " + response.StatusCode, "SelectQrPatchCompleted", "WebApiManager");
            YucoDebugger.instance.LogError("Facial API: Error detail = " + response.ErrorMessage, "SelectQrPatchCompleted", "WebApiManager");
            return;
        }
        Debug.Log("Facial API: Select QR for id = " + playerId + " succeeded.");
    }

    // === QR Uploaded functions =================================================================
    // ----------------------------------------------------------------
    public void RecordQrUploaded()
    {
        StartCoroutine(QrUploadedPatch(QrUploadedPatchCompleted));
    }

    // ----------------------------------------------------------------
    private IEnumerator QrUploadedPatch(Action<IRestResponse<string>> callback = null)
    {
        RestRequest request = new RestRequest(GetApiUrl(API_URL.QR_UPLOADED), Method.PATCH);
        YucoDebugger.instance.Log("Facial API: QR uploaded for id = " + playerId + ": " + GetApiUrl(API_URL.QR_UPLOADED), "QrUploadedPatch", "WebApiManager");

        yield return request.Request.SendWebRequest();
        request.GetText(callback);
    }

    // ----------------------------------------------------------------
    private void QrUploadedPatchCompleted(IRestResponse<string> response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            YucoDebugger.instance.LogError("Facial API: QR uploaded for id = " + playerId + ", StatusCode = " + response.StatusCode, "QrUploadedPatchCompleted", "WebApiManager");
            YucoDebugger.instance.LogError("Facial API: Error detail = " + response.ErrorMessage, "QrUploadedPatchCompleted", "WebApiManager");
            return;
        }
        Debug.Log("Facial API: QR uploaded for id = " + playerId + " succeeded.");
    }

    // === SELECT Email functions =================================================================
    // ----------------------------------------------------------------
    public void RecordSelectEmail()
    {
        StartCoroutine(SelectEmailPatch(SelectEmailPatchCompleted));
    }

    // ----------------------------------------------------------------
    private IEnumerator SelectEmailPatch(Action<IRestResponse<string>> callback = null)
    {
        RestRequest request = new RestRequest(GetApiUrl(API_URL.SELECT_EMAIL), Method.PATCH);
        YucoDebugger.instance.Log("Facial API: Select Email for id = " + playerId + ": " + GetApiUrl(API_URL.SELECT_EMAIL), "SelectEmailPatch", "WebApiManager");

        yield return request.Request.SendWebRequest();
        request.GetText(callback);
    }

    // ----------------------------------------------------------------
    private void SelectEmailPatchCompleted(IRestResponse<string> response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            YucoDebugger.instance.LogError("Facial API: Select Email for id = " + playerId + ", StatusCode = " + response.StatusCode, "SelectEmailPatchCompleted", "WebApiManager");
            YucoDebugger.instance.LogError("Facial API: Error detail = " + response.ErrorMessage, "SelectEmailPatchCompleted", "WebApiManager");
            return;
        }
        YucoDebugger.instance.Log("Facial API: Select Email for id = " + playerId + " succeeded.");
    }

    // === Send Email functions =================================================================
    // ----------------------------------------------------------------
    public void SendEmail(Language language, byte[] photoBytes, string address1, string address2 = "")
    {
        StartCoroutine(SendEmailPatch(language, photoBytes, address1, address2));
    }

    // ----------------------------------------------------------------
    private IEnumerator SendEmailPatch(Language language, byte[] photoBytes, string address1, string address2 = "")
    {
        YucoDebugger.instance.Log("Facial API: Send Email for id = " + playerId + ": " + GetApiUrl(API_URL.SEND_EMAIL), "SendEmailPatch", "WebApiManager");
        YucoDebugger.instance.Log("Facial API: Send Email language = " + language + ", address1 = " + address1, "SendEmailPatch", "WebApiManager");
        YucoDebugger.instance.Log("Facial API: Send Email address2 = " + address2, "SendEmailPatch", "WebApiManager");
        
        // Upload to server + Get QR code
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("lang", (LanguageManager.instance.currentLanguage == Language.Chinese) ? "c" : "e");
        wwwForm.AddField("email1", address1);
        if (address2 != "")
            wwwForm.AddField("email2", address2);
        wwwForm.AddBinaryData("attachedImage", photoBytes, "photo.jpg", "multipart/form-data");

        WWW wwwRequest = new WWW(GetApiUrl(API_URL.SEND_EMAIL), wwwForm);

        yield return wwwRequest;
        if (!string.IsNullOrEmpty(wwwRequest.error))
        {
            // ***** TO-DO: URL error, should go to Error page
            YucoDebugger.instance.LogError("Facial API: Send email for id = " + playerId + ", Error = " + wwwRequest.error + ": " + wwwRequest.text);
        }
        else
        {
            YucoDebugger.instance.Log("Facial API: Send email for id = " + playerId + ", email sent to " + address1 + "," + address2);

        }
    }

    // ======================================================================================================================================================================
    //   VisitInfo API functions 
    // ======================================================================================================================================================================

    // === CREATE functions =================================================================
    // ----------------------------------------------------------------
    public void RecordNewVisit(Language language)
    {
        visitEntry.lang_tchi = (language == Language.Chinese) ? 1 : 0;
        visitEntry.lang_eng = (language == Language.English) ? 1 : 0;

        StartCoroutine(CreateVisitPost(CreateVisitPostCompleted));
    }

    // ----------------------------------------------------------------
    private IEnumerator CreateVisitPost(Action<IRestResponse<string>> callback = null)
    {
        RestRequest request = new RestRequest(GetVisitApiUrl(VISIT_API_URL.CREATE), Method.POST);

        string jsonString = JsonMapper.ToJson(visitEntry);

        request.AddHeader("Content-Type", "application/json");
        request.AddBody(jsonString);

        YucoDebugger.instance.Log("Visit API: create visit entry: " + GetVisitApiUrl(VISIT_API_URL.CREATE), "CreateVisitPost", "WebApiManager");

        yield return request.Request.SendWebRequest();
        request.GetText(callback);
    }

    // ----------------------------------------------------------------
    private void CreateVisitPostCompleted(IRestResponse<string> response)
    {
        if (response.IsError)
        {
            YucoDebugger.instance.LogError("Visit API: create visit entry Error, StatusCode = " + response.StatusCode, "CreateVisitPostCompleted", "WebApiManager");
            YucoDebugger.instance.LogError("Visit API: Error detail = " + response.ErrorMessage, "CreateVisitPostCompleted", "WebApiManager");
            return;
        }
        YucoDebugger.instance.Log("Visit API:  create visit entry succeeded. id = " + response.Content);
        visitPlayerId = response.Content;
    }

    // === Set Timeout functions =================================================================
    // ----------------------------------------------------------------
    public void RecordVisitTimeout()
    {
        StartCoroutine(VisitTimeoutPatch(VisitTimeoutPatchCompleted));
    }

    // ----------------------------------------------------------------
    private IEnumerator VisitTimeoutPatch(Action<IRestResponse<string>> callback = null)
    {
        RestRequest request = new RestRequest(GetVisitApiUrl(VISIT_API_URL.SET_TIMEOUT), Method.PATCH);
        YucoDebugger.instance.Log("Visit API: Set timeout for id = " + visitPlayerId + ": " + GetVisitApiUrl(VISIT_API_URL.SET_TIMEOUT), "VisitTimeoutPatch", "WebApiManager");

        yield return request.Request.SendWebRequest();
        request.GetText(callback);
    }

    // ----------------------------------------------------------------
    private void VisitTimeoutPatchCompleted(IRestResponse<string> response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
        {
            YucoDebugger.instance.LogError("Visit API: Set timeout for id = " + visitPlayerId + ", StatusCode = " + response.StatusCode, "VisitTimeoutPatchCompleted", "WebApiManager");
            YucoDebugger.instance.LogError("Visit API: Error detail = " + response.ErrorMessage, "VisitTimeoutPatchCompleted", "WebApiManager");
            return;
        }
        Debug.Log("Visit API: Set timeout for id = " + visitPlayerId + " succeeded.");
    }


    // ===================================================================================
    //   URL functions 
    // ===================================================================================
    // ----------------------------------------------------------------
    private string GetApiUrl(API_URL url_type)
    {
        switch(url_type)
        {
            case API_URL.CREATE:
                return settingMgr.apiBaseUrl + "create";                
            case API_URL.SELECT_QR:
                return settingMgr.apiBaseUrl + playerId + "/qr";
            case API_URL.QR_UPLOADED:
                return settingMgr.apiBaseUrl + playerId + "/qrupload";
            case API_URL.SELECT_EMAIL:
                return settingMgr.apiBaseUrl + playerId + "/email";
            case API_URL.SEND_EMAIL:
                return settingMgr.apiBaseUrl + playerId + "/sendmail";
            default:
                return "";
        }
    }

    // ----------------------------------------------------------------
    private string GetVisitApiUrl(VISIT_API_URL url_type)
    {
        switch (url_type)
        {
            case VISIT_API_URL.CREATE:
                return settingMgr.apiVisitinfoUrl + "create";
            case VISIT_API_URL.SET_TIMEOUT:
                return settingMgr.apiVisitinfoUrl + visitPlayerId + "/timeout";
            default:
                return "";
        }
    }

}
