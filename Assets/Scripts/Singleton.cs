using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance 
    { 
        get 
        { 
            if(instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if(instance == null)
                {
                    instance = new GameObject().AddComponent<T>();
                }
            }
            return instance;
        } 
    }

    protected void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(this);
        }
    }
}
