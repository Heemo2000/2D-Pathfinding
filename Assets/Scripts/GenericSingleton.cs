using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T : GenericSingleton<T>
{
    private static T _instance;

    public static T Instance { get => _instance;}

    protected virtual void Awake() {
        if(Instance == null)
        {
            _instance = this as T;
            //DontDestroyOnLoad(this);
            return;
        }
        
        Destroy(gameObject);
    }
}
