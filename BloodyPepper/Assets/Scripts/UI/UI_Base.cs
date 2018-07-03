using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Base : MonoBehaviour
{
    private bool isInitialized = false;
    public  bool Initialized { get { return isInitialized; } private set { isInitialized = value; } }


    public virtual void Initialize()
    {
        if (Initialized)
            return;

        Initialized = true;
    }

    public virtual void Open()
    {
        Initialize();
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        GameObject.Destroy(gameObject);
    }

}
