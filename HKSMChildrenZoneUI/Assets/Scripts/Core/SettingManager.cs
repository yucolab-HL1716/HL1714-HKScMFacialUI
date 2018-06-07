using UnityEngine;
using System;
using System.IO;

public class SettingManager : MonoSingleton<SettingManager>
{
    // === Public params ===============================
    // Debugger
    private YucoDebugger debugger;

    // GUI events and static actions
    public delegate void SettingGUIEventDlgt(object sender, SettingGUIEventArgs e);
    public event SettingGUIEventDlgt GUIBtnClicked;
    
    // INI and resources
    private string iniInternal_path = "data/prefs";
    private IniFile iniInternal;
    private string iniCms_path = "data/program_settings";
    private IniFile iniCms;
    // INI parameters (Internal)
    public string computerId = "FacialPC_1";
    public string ip = "127.0.0.1";
    public int port = 2838;
    public bool debugMode = false;
    public bool playBgm = true;
    public float arrowX = 0;
    public string apiBaseUrl = "http://localhost/hksm_childrenzone/api/facials/";
    public string apiVisitinfoUrl = "http://localhost/hksm_childrenzone/api/visitinfos/";
    public string uploadQRUrl = "http://localhost/hksmcz_facial/uploadImage.php";
    // INI parameters (CMS)
    public int idleTime = 300;
    public int stillHereCountdownTime = 10;
    public bool isDefaultEng = false;
    public int maxStickerCount = 20;
    public int photoQuality = 95;
    public float dragMinimumScale = 0.67f;

    // file paths
    public string debugLogPath;
    public string fullResourcesPath;
    public string photoFolderPath;
    
    public int stageWidth = 1920;
    public int stageHeight = 1080;
    public Vector2 captureAnchorPosition = new Vector2(229, 250);

    // ----------------------------------------------------------------
    public override void Init()
    {
        // Debugger
        debugger = YucoDebugger.instance;

        debugLogPath = "C:/yucolab/" + Application.productName + "/Log";
        fullResourcesPath = Application.streamingAssetsPath + "/";

        iniInternal_path = fullResourcesPath + "internal/settings";
        iniInternal = new IniFile();

        iniCms_path = fullResourcesPath + "facialUI_cms/settings";
        iniCms = new IniFile();
    }
    
    
    // === INI load/save functions ========================================================================================================================================
    // ----------------------------------------------------------------
    // Load internal settings from INI file
    public void LoadInternalSettings()
    {
        // =======  Load internal INI   =======
        YucoDebugger.instance.Log("internal prefs.ini loading is about to start...", "LoadIniSettings", "SettingManager");
        iniInternal.Load(iniInternal_path);
        // Parameters loading (internal)
        computerId = iniInternal.Get("computerId", "FacialPC_1");
        debugMode = iniInternal.Get("debugMode", true);
        playBgm = iniInternal.Get("playBgm", true);
        debugLogPath = iniInternal.Get("debugLogPath", "C:/yucolab/" + Application.productName + "/Log");
        photoFolderPath = iniInternal.Get("photoFolderPath", "C:/yucolab/" + Application.productName + "/Photos");
        ip = iniInternal.Get("ip", "127.0.0.1");
        port = iniInternal.Get("port", 2838);
        arrowX = iniInternal.Get("arrowX", 0);
        apiBaseUrl = iniInternal.Get("apiBaseUrl", "http://localhost/hksm_childrenzone/api/facials/");
        apiVisitinfoUrl = iniInternal.Get("apiVisitinfoUrl", "http://localhost/hksm_childrenzone/api/visitinfos/");
        uploadQRUrl = iniInternal.Get("uploadQRUrl", "http://localhost/hksmcz_facial/uploadImage.php");

        
        YucoDebugger.instance.Log("iniInternal_path = " + iniInternal_path, "LoadIniSettings", "SettingManager");
        YucoDebugger.instance.Log("uploadQRUrl = " + uploadQRUrl, "LoadIniSettings", "SettingManager");
        YucoDebugger.instance.Log("apiBaseUrl = " + apiBaseUrl, "LoadIniSettings", "SettingManager");
        YucoDebugger.instance.Log("apiVisitinfoUrl = " + apiVisitinfoUrl, "LoadIniSettings", "SettingManager");

        // Setup after loading
        if (!Directory.Exists(photoFolderPath))
        {
            Directory.CreateDirectory(photoFolderPath);
        }
        SetDebugMode(debugMode);
        debugger.SetLogFolder(debugLogPath);
        YucoDebugger.instance.Log("internal prefs.ini loaded!", "LoadIniSettings", "SettingManager");

        // =======   Load CMS INI    =======
        LoadCmsSettings();
    }

    // ----------------------------------------------------------------
    // Load CMS settings from INI file
    public void LoadCmsSettings()
    {
        YucoDebugger.instance.Log("CMS prefs.ini loading is about to start...", "LoadIniSettings", "SettingManager");
        iniCms.Load(iniCms_path);
        // Parameters loading (CMS)
        idleTime = iniCms.Get("idleTime", 180);
        stillHereCountdownTime = iniCms.Get("stillHereCountdownTime", 10);
        isDefaultEng = iniCms.Get("isDefaultEng", false);
        maxStickerCount = iniCms.Get("maxStickerCount", 20);
        photoQuality = iniCms.Get("photoQuality", 95);
        dragMinimumScale = iniCms.Get("dragMinimumScale", 0.67f);
        YucoDebugger.instance.Log("CMS prefs.ini loaded!", "LoadIniSettings", "SettingManager");
    }

    // === Player config functions ========================================================================================================================================
    // ----------------------------------------------------------------
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            ToggleDebugMode();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    // ----------------------------------------------------------------
    // Setup player config
    public void ToggleDebugMode()
    {
        debugMode = !debugMode;
        SetDebugMode(debugMode);
        debugger.debugMode = debugMode;
    }

    // ----------------------------------------------------------------
    public void SetDebugMode(bool _debug)
    {
        debugMode = _debug;
        debugger.debugMode = debugMode;
        if (debugMode)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }
    }

    // ----------------------------------------------------------------
	private string GetDate()
    {
        return string.Format("{0:yyyyMMdd}", DateTime.Now);
    }

    // ----------------------------------------------------------------
    public string GetTimestampFilename()
    {
        return photoFolderPath + "/" + string.Format("{0:yyyyMMdd_HHmmss}.jpg", DateTime.Now);
    }
}

/// === SettingGUIEventArgs Class ========================================================================================================================================
/// <summary>
/// Implements a custom EventArgs class for passing GUI event.
/// </summary>
/// ======================================================================================================================================================================
public class SettingGUIEventArgs : EventArgs
{
    protected string target;

    // ----------------------------------------------------------------
    public string Target
    {
        get { return target; }
    }

    // ----------------------------------------------------------------
    public SettingGUIEventArgs(string _target)
    {
        target = _target;
    }
}
