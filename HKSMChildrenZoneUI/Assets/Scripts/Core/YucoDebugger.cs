using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class YucoDebugger : MonoSingleton<YucoDebugger>
{
	public string logFolder;
	public string logPath;
	
	private StreamWriter streamwriter = null;
	private string message = "";
    private string separator = " ::::: ";

    public bool debugMode = false;
	
	// === Core functions ========================================================================================================================================
    // ----------------------------------------------------------------
    public override void Init()
	{
        logFolder = "C:/yucolab/" + Application.productName + "/Log";
        logPath = "C:/yucolab/" + Application.productName + "/Log/log.txt";
    }

    // ----------------------------------------------------------------
	private void OnApplicationQuit()
    {
        if(streamwriter != null)
		{
			streamwriter.Close();
			streamwriter.Dispose();
		}
    }
	
	// === Log functions ========================================================================================================================================
    // ----------------------------------------------------------------
	// Setup log folder and file
	public void SetLogFolder(string path)
	{
		// Set and check log folder, create one if not exists
		logFolder = path;
		if (!Directory.Exists(logFolder))
		{
    		Directory.CreateDirectory(logFolder);
		}
				
		// Set log file path
		logPath = logFolder + "/log-" + GetDate() + ".txt";
		streamwriter = new StreamWriter(logPath, true);
		WriteToLog("===================== New application start =======================");
		
		// Check and delete log that are older than your mama (180 days)
		ClearOldLogs();
	}

    // ----------------------------------------------------------------
	// Check log file and delete those older than your auntie (180 days)
	private void ClearOldLogs()
	{		
		// LINQ
		(from f in new DirectoryInfo(logFolder).GetFiles()
		where f.CreationTime < DateTime.Now.Subtract(TimeSpan.FromDays(180))
		select f).ToList()
		.ForEach(f => DeleteOldFile(f));	
	}

    // ----------------------------------------------------------------
	private void DeleteOldFile(FileInfo fileinfo)
	{
		Log ("Delete file " + fileinfo.FullName, "DeleteOldFile", "YucoDebugger");
		fileinfo.Delete();
	}

    // ----------------------------------------------------------------
	// Write to log txt file via StreamWriter
	private void WriteToLog(string str)
	{
		if(streamwriter != null)
		{
			streamwriter.WriteLine(str);
		}
	}
	
	// === Debug functions ========================================================================================================================================
    // ----------------------------------------------------------------
    private string GetTimeStamp()
    {
        return string.Format("{0:HH:mm:ss-fff}", DateTime.Now);
    }

    // ----------------------------------------------------------------
	private string GetDate()
    {
        return string.Format("{0:yyyyMMdd}", DateTime.Now);
    }

    // ----------------------------------------------------------------
	public void LogDebug(string str, string functionName = "", string className = "")
    {
		if(debugMode)
		{
			message = GetTimeStamp() + " [DEBUG] " + GetFunctionString(functionName, className) + str;
        	Debug.Log(message);
			WriteToLog(message);
		}
    }

    // ----------------------------------------------------------------
    public void Log(string str, string functionName = "", string className = "")
    {
		message = GetTimeStamp() + " [LOG] " + GetFunctionString(functionName, className) + str;
        Debug.Log(message);
		WriteToLog(message);
    }

    // ----------------------------------------------------------------
    public void LogError(string str, string functionName = "", string className = "")
    {
		message = GetTimeStamp() + " [ERROR] " + GetFunctionString(functionName, className) + str;
        Debug.LogError(message);
		WriteToLog(message);
    }

    // ----------------------------------------------------------------
	public void LogWarning(string str, string functionName = "", string className = "")
	{
		message = GetTimeStamp() + " [WARN] " + GetFunctionString(functionName, className) + str;
        Debug.LogWarning(message);
		WriteToLog(message);
        
	}

    // ----------------------------------------------------------------
    private string GetFunctionString(string functionName = "", string className = "")
    {
        string msg = "";
        if(className != "")
        {
            msg += className + ".";
        }
        if (functionName != "")
        {
            msg += functionName + "()" + separator;
        }

        return msg;
    }
}

