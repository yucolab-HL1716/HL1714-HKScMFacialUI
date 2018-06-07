using UnityEngine;

public abstract class MonoPersistentSingleton<T> : MonoBehaviour where T : MonoPersistentSingleton<T>
{
    private static T m_Instance = null;
    public static T instance
    {
        get
        {
            // Instance required for the first time, we look for it
            if (m_Instance == null)
            {
                m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

                // Object not found, we create a temporary one
                if (m_Instance == null)
                {
                    //Debug.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");
                    m_Instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();

                    // Problem during the creation, this should not happen
                    if (m_Instance == null)
                    {
                        Debug.LogError("Problem during the creation of " + typeof(T).ToString());
                    }
                }
                m_Instance.Init();
            }
            return m_Instance;
        }
    }
    
    private void Awake()
    {
        // No monobehaviour request the instance in an awake function executing before this one
        if (m_Instance == null)
        {
            m_Instance = this as T;
            m_Instance.Init();

            // Keep this gameobject persistent
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // singleton already exists. Delete this object
            Destroy(gameObject);
        }
    }

    // This function is called when the instance is used the first time
    // Put all the initializations you need here, as you would do in Awake
    public virtual void Init() { }

    // Make sure the instance isn't referenced anymore when the user quit, just in case.
    private void OnApplicationQuit()
    {
        m_Instance = null;
    }
}