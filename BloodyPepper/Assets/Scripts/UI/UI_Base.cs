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
        transform.localPosition = UIManager.Instance.UI_POS_HIDE;
    }

    public virtual void Open()
    {
        Initialize();

        transform.SetAsLastSibling();
        transform.localPosition = UIManager.Instance.UI_POS_SHOW;
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        GameObject.Destroy(gameObject);
    }

    public bool IsVisible { get { return Mathf.Abs(transform.localPosition.z) < Mathf.Abs(UIManager.Instance.UI_POS_HIDE.z); } }
         

}
