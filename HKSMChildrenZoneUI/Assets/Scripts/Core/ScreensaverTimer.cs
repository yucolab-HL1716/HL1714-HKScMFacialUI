using UnityEngine;
using System.Collections;

public class ScreensaverTimer: MonoBehaviour
{
    public float timeLimit = 10;
    public float idleTime = 0.0f;
    public bool started = false;
    public bool timesup = false;

    // ----------------------------------------------------------------
    void Start()
	{
	}

    // ----------------------------------------------------------------
    void Update()
	{
		if(started && !timesup)
		{
			idleTime += Time.deltaTime;
            if (idleTime >= timeLimit) 
			{
                idleTime = 0.0f;
                started = false;
                timesup = true;
			}
		}
	}

    // ----------------------------------------------------------------
    public void SetTimeLimit(float limit)
    {
        timeLimit = limit;
        YucoDebugger.instance.Log("Screensaver limit is set to " + timeLimit, "SetTimeLimit", "ScreensaverTimer");
    }

    // ----------------------------------------------------------------
    public void StartTimer()
	{
		idleTime = 0.0f;
		started = true;
	}
    
    // ----------------------------------------------------------------
    public void ResetTimer()
	{
		idleTime = 0.0f;
		started = false;
		timesup = false;
	}

    // ----------------------------------------------------------------
    public void ResetIdleTime()
	{
		idleTime = 0.0f;
	}

    // ----------------------------------------------------------------
    public bool isTimesup()
	{
		return timesup;
	}	
}

