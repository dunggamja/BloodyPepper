using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public const    float   POS_Z_HIDE  = -40000f;
    public readonly Vector3 UI_POS_HIDE = new Vector3(0f, 0f, POS_Z_HIDE);
    public readonly Vector3 UI_POS_SHOW = Vector3.zero;
    

    public static UIManager Instance { get; private set; }

	// Use this for initialization
	void Awake ()
    {
        Instance = this;
        DontDestroyOnLoad(this);
	}


    private List<UI_Base> listOpenUI = new List<UI_Base>(32);


    public T OpenUI<T>() where T : UI_Base
    {
        T openUI = FindUI<T>();

        //새로 생성.
        if(null == openUI)
        {
            openUI = CreateUI<T>();
        }

        if (null != openUI)
            openUI.Open();            

        return openUI;
    }

    private T LoadUIPrefab<T>() where T : UI_Base
    {
        T openUI = null;
        string typeName = typeof(T).ToString();
        string path = string.Format("Prefabs/UI/{0}", typeName);
        GameObject oriUI = Resources.Load<GameObject>(path);
        if (null != oriUI)
        {
            GameObject newUI = GameObject.Instantiate<GameObject>(oriUI, transform);
            openUI = newUI.GetComponent<T>();            
        }

        return openUI;
    }


    public void CloseUI<T>() where T : UI_Base
    {
        T closeUI = listOpenUI.Find((UI_Base item) =>
        {
            if (item.GetType() == typeof(T))
                return true;
            return false;
        }) as T;


        if (null != closeUI)
        {
            listOpenUI.Remove(closeUI);
            closeUI.Close();
        }
    }

    public T FindUI<T>() where T : UI_Base
    {
        T openUI = listOpenUI.Find((UI_Base item) =>
        {
            if (item.GetType() == typeof(T))
                return true;
            return false;
        }) as T;

        return openUI;
    }

    public T CreateUI<T>() where T : UI_Base
    {
        T openUI = FindUI<T>();
        if (null != openUI)
            return openUI;

        
        openUI = LoadUIPrefab<T>();
        if (null != openUI)
        {
            listOpenUI.Add(openUI);
            openUI.Initialize();
        }

        return openUI;
    }



    public void CloseAllUI()
    {
        for (int i = 0; i < listOpenUI.Count; ++i)
        {
            listOpenUI[i].Close();
        }
        listOpenUI.Clear();
    }
}
