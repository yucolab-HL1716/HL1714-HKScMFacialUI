using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using DoozyUI;
using TMPro;
using LitJson;
using TouchScript;

using Yucolab;


public class GameModeChangeEventArgs : EventArgs
{
    public GameMode currentGameMode;

    // ---------------------------------------------------------------
    public GameModeChangeEventArgs(GameMode gameMode)
    {
        currentGameMode = gameMode;
    }
}

public class GameRoot : MonoSingleton<GameRoot>
{
    // Singleton
    private SettingManager settingManager;
    private YucoDebugger debugger;
    private WebApiManager webApiMgr;
    // UDP sender
    [HideInInspector]
    public UdpSender udpSender;
    // UDP receiver
    [HideInInspector]
    public UdpReceiver udpReceiver;
    // QR Code manager
    public QRCodeManager qrManager;
    // Screensaver Timer
    public ScreensaverTimer screensaverTimer;
    // Still Here Pop Up
    public StillHerePopUpWindow stillHerePopUp;
    // Background Color acrroding to game mode
    public BackgroundColorByGame colorBg;
    // Debug text panel
    public GameObject debugPanel;
    public Text text_debug;
    private bool bShowDebugPanel = false;

    // Task queue for delagates
    public delegate void Task();
    private Queue<Task> TaskQueue = new Queue<Task>();
    private object queueLock = new object();

    // Game properties
    public delegate void GameModeChangeEventDlgt(object sender, GameModeChangeEventArgs e);
    public event GameModeChangeEventDlgt GameModeChanged = delegate { };
    public GameMode gameMode;

    // UI
    public string countdownString = "";
    public List<TextMeshProUGUI> texts_countdown;
    private Texture2D finalPhotoTex2d;
    public List<RectTransform> lookCamArrows;
    public List<SideBar> sideBars;

    // SFX
    public AudioSource audioSrc_bgm;
    public AudioSource audioSrc;
    public AudioClip audio_camShutter;
    public AudioClip audio_countdown;

    // === Core functions ========================================================================================================================================
    // ----------------------------------------------------------------
    void Start()
    {
        // Setting Manager and XML loading
        settingManager = SettingManager.instance;
        settingManager.LoadInternalSettings();
        debugger = YucoDebugger.instance;
        // UDP sender
        udpSender = GetComponent<UdpSender>();
        udpSender.SetServerInfo(settingManager.ip, 9999);
        // UDP receiver
        udpReceiver = GetComponent<UdpReceiver>();
        udpReceiver.StartListening(9998);
        udpReceiver.UdpReceived += OnUdpReceived;
        // Screensaver Timer
        screensaverTimer.SetTimeLimit(settingManager.idleTime);
        stillHerePopUp.SetTimeLimit(settingManager.stillHereCountdownTime);

        // Play BGM?
        if(settingManager.playBgm == true)
        {
            audioSrc_bgm.Play();
        }

        // Arrow X
        foreach(RectTransform arrow in lookCamArrows)
        {
            arrow.anchoredPosition = new Vector2(settingManager.arrowX, 0);
        }

        // Web API Manager
        webApiMgr = WebApiManager.instance;

        // Reset Game
        ResetGame();

        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersPressed += pointersPressedHandler;
        }
    }

    // ----------------------------------------------------------------
    private void pointersPressedHandler(object sender, PointerEventArgs e)
    {
        if (gameMode != GameMode.Nil)
        {
            if (!screensaverTimer.isTimesup())
            {
                screensaverTimer.ResetIdleTime();
            }
        }
    }

    // ----------------------------------------------------------------
    void OnApplicationQuit()
    {
        if (udpReceiver != null)
        {
            udpReceiver.StopListening();
            udpReceiver.UdpReceived -= OnUdpReceived;
        }
    }

    // ----------------------------------------------------------------
    void Update()
    {
        // Handle task queue from delegates
        lock (queueLock)
        {
            if (TaskQueue.Count > 0)
                TaskQueue.Dequeue()();
        }

        // Debug Panel
        if (Input.GetKeyUp(KeyCode.B))
        {
            bShowDebugPanel = !bShowDebugPanel;
            debugPanel.SetActive(bShowDebugPanel);
        }
        if (bShowDebugPanel)
        {
            text_debug.text = "Screensaver time = " + screensaverTimer.idleTime.ToString("F2");
            text_debug.text += "\nTouch count = " + Input.touchCount.ToString();
            text_debug.text += "\nscreensaverTimer.isTimesup = " + screensaverTimer.isTimesup().ToString();
            text_debug.text += "\nGameMode = " + gameMode.ToString();
            text_debug.text += "\nInput.GetMouseButton(0) = " + Input.GetMouseButton(0).ToString();
            text_debug.text += "\nInput.GetMouseButton(1) = " + Input.GetMouseButton(1).ToString();
            text_debug.text += "\nInput.GetMouseButton(2) = " + Input.GetMouseButton(2).ToString();
        }

        if (gameMode != GameMode.Nil)
        {
            if (!screensaverTimer.isTimesup())
            {
                /*if (Input.touchCount > 0)
                {
                    screensaverTimer.ResetIdleTime();
                }

                if(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
                {
                    screensaverTimer.ResetIdleTime();
                }*/
                
            }
            else if (!stillHerePopUp.isVisible)
            {
                // Open still here window
                stillHerePopUp.ShowWindow();
                UIManager.HideUiElement("PopUp_BackToHome", "FacialGameMenu");
            }
        }
    }

    // ----------------------------------------------------------------
    /*public void OnGUI()
    {
        if (gameMode != GameMode.Nil)
        {
            if (!screensaverTimer.isTimesup())
            {
                if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseUp)
                {
                    screensaverTimer.ResetIdleTime();
                }
            }
            else if (!stillHerePopUp.isVisible)
            {
                // Open still here window
                stillHerePopUp.ShowWindow();
            }
        }
    }*/

    // ----------------------------------------------------------------
    public void ResetGame()
    {
        gameMode = GameMode.Nil;
        GameModeChanged(this, new GameModeChangeEventArgs(gameMode));

        List<UIElement> visibleElmt = UIManager.GetVisibleUIElements();
        visibleElmt.ForEach(em => em.Hide(true, true));
        UIManager.ShowUiElement("MainMenu", "FacialGame");

        colorBg.UpdateBackgroundColor(gameMode);
        sideBars.ForEach(b => b.Reset());

        Game1Manager.instance.ResetGame();
        Game2Manager.instance.ResetGame();
        Game3Manager.instance.ResetGame();

        KeyboardManager.instance.ResetKeyboard();
        StickerManager.instance.ClearAllStickers();
        qrManager.ResetQR();

        SendUdpMessage("reset");

        finalPhotoTex2d = null;
        Resources.UnloadUnusedAssets();

        screensaverTimer.ResetTimer();

        //LanguageManager.instance.SetLanguage((settingManager.isDefaultEng) ? Language.English : Language.Chinese);
        webApiMgr.Reset();

        // Load settings again each round
        settingManager.LoadCmsSettings();
    }

    // ----------------------------------------------------------------
    public void ResetGameOnTimeout()
    {
        // Record new visit entry
        webApiMgr.RecordVisitTimeout();
        ResetGame();
    }

    // === Photo saving functions ================================================================================================================================
    // ----------------------------------------------------------------
    public void SavePhotoToFile(Texture2D texture2D)
    {
        finalPhotoTex2d = texture2D;

        var bytes = texture2D.EncodeToJPG(settingManager.photoQuality);
        string filename = settingManager.GetTimestampFilename();
        FileStream file = File.Open(filename, FileMode.Create);
        BinaryWriter binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();

        // tell OF to load
        SendUdpMessage("photo", filename);
    }

    // === UITrigger functions ================================================================================================================================
    // ----------------------------------------------------------------
    public void OnGameButtonPressed(int _gameMode)
    {
        gameMode = (GameMode)_gameMode;
        GameModeChanged(this, new GameModeChangeEventArgs(gameMode));

        SendUdpMessage("startGame", _gameMode.ToString());
        sideBars[(int)gameMode].SetIcon(0);

        colorBg.UpdateBackgroundColor(gameMode);

        screensaverTimer.StartTimer();

        // Record new gameplay entry
        webApiMgr.RecordNewGameplay(_gameMode, LanguageManager.instance.currentLanguage);
        // Record new visit entry
        webApiMgr.RecordNewVisit(LanguageManager.instance.currentLanguage);
    }

    // ----------------------------------------------------------------
    public void OnStartCamP1Triggered()
    {
        SendUdpMessage("startCam", "1");
        sideBars[(int)gameMode].SetIcon(0);
    }

    // ----------------------------------------------------------------
    public void OnStartCamP2Triggered()
    {
        SendUdpMessage("startCam", "2");
        sideBars[(int)gameMode].SetIcon(0);
    }

    // ----------------------------------------------------------------
    public void OnEmailShareClicked()
    {
        sideBars[(int)gameMode].SetIcon(3);
        if (gameMode == GameMode.GAME2)
        {
            UIManager.HideUiElement("ShareOption", "FacialGame");
            UIManager.ShowUiElement("EmailInput_2P", "FacialGame");
            Keyboard2PManager.instance.ResetKeyboard();
        }
        else
        {
            UIManager.HideUiElement("ShareOption", "FacialGame");
            UIManager.ShowUiElement("EmailInput", "FacialGame");
            KeyboardManager.instance.ResetKeyboard();
        }

        // Record select email
        webApiMgr.RecordSelectEmail();
    }

    // ----------------------------------------------------------------
    public void OnDecorationArrowPressed()
    {
        if (gameMode == GameMode.GAME1)
        {
            Game1Manager.instance.OnDecorationArrowPressed();
        }
        else if (gameMode == GameMode.GAME2)
        {
            Game2Manager.instance.OnDecorationArrowPressed();
        }
        else if (gameMode == GameMode.GAME3)
        {
            Game3Manager.instance.OnDecorationArrowPressed();
        }
    }

    // ----------------------------------------------------------------
    public void OnLanguageButtonPressed()
    {
        LanguageManager.instance.ToggleLanguage();
    }

    // ----------------------------------------------------------------
    public void RestartScreensaverTimer()
    {
        screensaverTimer.ResetTimer();
        screensaverTimer.StartTimer();
    }

    // === Share Option functions ========================================================================================================================================
    // ----------------------------------------------------------------
    public void OnShareQRButtonPressed()
    {
        YucoDebugger.instance.Log("Prepare to upload photo for QR", "OnShareQRButtonPressed", "GameRoot");

        // Record select QR
        webApiMgr.RecordSelectQR();

        StartCoroutine(UploadImageForQR());
    }

    // ----------------------------------------------------------------
    private IEnumerator UploadImageForQR()
    {
        string filename = settingManager.computerId + string.Format("{0:HHmmssfff}", DateTime.Now);

        // Create HMAC string
        Encoding encoding = Encoding.UTF8;
        var keyByte = encoding.GetBytes("HKSM Children Zone Facial");
        string hmacString = "";
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            hmacsha256.ComputeHash(encoding.GetBytes(filename));
            hmacString = ByteToString(hmacsha256.Hash);
        }
        var photoBytes = finalPhotoTex2d.EncodeToJPG(settingManager.photoQuality);

        // Upload to server + Get QR code
        WWWForm form = new WWWForm();
        form.AddField("s", hmacString);
        form.AddField("d", filename);
        form.AddBinaryData("fileToUpload", photoBytes, filename + ".jpg", "image/jpg");

        WWW wwwRequest = new WWW(settingManager.uploadQRUrl, form);

        YucoDebugger.instance.Log("uploadQRUrl = " + settingManager.uploadQRUrl, "UploadImageForQR", "GameRoot");
        yield return wwwRequest;
        
        if (!string.IsNullOrEmpty(wwwRequest.error))
        {
            // ***** TO-DO: URL error, should go to Error page
            YucoDebugger.instance.LogError(wwwRequest.error, "UploadImageForQR", "GameRoot");
        }
        else
        {
            YucoDebugger.instance.Log(wwwRequest.text, "UploadImageForQR", "UploadImage");
            JsonData jsonData = JsonMapper.ToObject(wwwRequest.text);

            if (jsonData["result"] != null)
            {
                if (jsonData["result"].ToString() == "success")
                {
                    YucoDebugger.instance.Log("Upload Succeeded: URL = " + jsonData["url"].ToString(), "UploadImageForQR", "GameRoot");
                    qrManager.GenerateQR(jsonData["url"].ToString());
                    webApiMgr.RecordQrUploaded();
                }
                else if (jsonData["result"].ToString() == "error")
                {
                    // ***** TO-DO: URL error, should go to Error page
                    YucoDebugger.instance.LogError("Upload error: " + jsonData["msg"].ToString(), "UploadImageForQR", "GameRoot");
                }
            }
        }
    }

    // ----------------------------------------------------------------
    static string ByteToString(byte[] buff)
    {
        string sbinary = "";
        for (int i = 0; i < buff.Length; i++)
            sbinary += buff[i].ToString("X2"); /* hex format */
        return sbinary.ToLower();
    }

    // ----------------------------------------------------------------
    public void EmailToVisitors(string address1, string address2 = "")
    {
        YucoDebugger.instance.Log("Prepare to send email to: " + address1, "EmailToVisitors", "GameRoot");
        if (address2 != "")
            YucoDebugger.instance.Log("Prepare to send email to: " + address2, "EmailToVisitors", "GameRoot");

        // Send email
        var photoBytes = finalPhotoTex2d.EncodeToJPG(settingManager.photoQuality);
        webApiMgr.SendEmail(LanguageManager.instance.currentLanguage, photoBytes, address1, address2);
    }


    // === UDP functions ========================================================================================================================================
    // ----------------------------------------------------------------
    public void SendUdpMessage(params string[] addresses)
    {
        string delimiter = "||";
        udpSender.SendUdpMessage(String.Join(delimiter, addresses));
    }

    // ----------------------------------------------------------------
    private void OnUdpReceived(object sender, UdpReceivedEventArgs e)
    {
        if (e.message.Length <= 0)
            return;

        string[] dataStrings = e.message.Split('_');
        string cmd = dataStrings[0];

        if (dataStrings.Length == 1)
        {
            // Hide countdown (nf = No face)
            if (cmd == "nf")
            {
                ScheduleTask(new Task(delegate
                {
                    countdownString = "";
                    texts_countdown.ForEach(tm => tm.text = "");
                }));
            }
            else if (cmd == "resume")
            {
                YucoDebugger.instance.Log("OF resume game...", "OnUdpReceived", "GameRoot");
                // ****** TO-DO: resume current status
            }
        }
        else if (dataStrings.Length > 1)
        {
            // Count down
            if (cmd == "cd")
            {
                screensaverTimer.ResetIdleTime();
                string duration = dataStrings[1];

                ScheduleTask(new Task(delegate
                {
                    if (countdownString != duration)
                    {
                        countdownString = duration;
                        audioSrc.PlayOneShot(audio_countdown);
                    }

                    texts_countdown.ForEach(tm => tm.text = duration);
                }));
            }
            // Photo taken with Player ID
            else if (cmd == "tk")
            {
                screensaverTimer.ResetIdleTime();

                int playerId = 1;
                int.TryParse(dataStrings[1], out playerId);
                playerId += 1;

                //Debug.Log("cmd = " + cmd + "_" + playerId.ToString());

                ScheduleTask(new Task(delegate
                {
                    audioSrc.PlayOneShot(audio_camShutter);

                    if (gameMode == GameMode.GAME1)
                    {
                        UIManager.HideUiElement("G1_TakePhoto", "FacialGame");
                        UIManager.ShowUiElement("G1_ConfirmPhoto", "FacialGame");
                    }
                    else if (gameMode == GameMode.GAME2)
                    {
                        UIManager.HideUiElement("G2_TakePhotoP" + playerId.ToString(), "FacialGame");
                        UIManager.ShowUiElement("G2_ConfirmPhotoP" + playerId.ToString(), "FacialGame");
                    }
                    else if (gameMode == GameMode.GAME3)
                    {
                        UIManager.HideUiElement("G3_TakePhoto", "FacialGame");
                        UIManager.ShowUiElement("G3_ConfirmPhoto", "FacialGame");
                    }
                }));
            }

        }
    }

    // === Task queue functions ================================================================================================================================
    // ----------------------------------------------------------------
    public void ScheduleTask(Task newTask)
    {
        lock (queueLock)
        {
            if (TaskQueue.Count < 100)
                TaskQueue.Enqueue(newTask);
        }
    }
}
