using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

	// Use this for initialization
	void Awake ()
    {
        Instance = this;
        DontDestroyOnLoad(this);
	}


    private List<UI_Base> listOpenUI = new List<UI_Base>(16);


    public void OpenUI<T>() where T : UI_Base
    {
        var openUI = listOpenUI.Find((UI_Base item) => 
        {
            if(item.GetType() == typeof(T))
                return true;
            return false;
        });

        //이미 존재할 경우.
        if (null != openUI)
        {
            //리스트의 맨뒤로 위치 변경
            listOpenUI.Remove(openUI);
            listOpenUI.Add(openUI);

            //UI 목록중 가장 앞에 표시되도록 수정.
            openUI.gameObject.transform.SetAsLastSibling();

            openUI.Open();
            return;
        }

        string typeName = typeof(T).ToString();
        string path = string.Format("Prefabs/UI/{0}", typeName);
        GameObject oriUI = Resources.Load<GameObject>(path);
        if (null != oriUI)
        {
            GameObject newUI = GameObject.Instantiate<GameObject>(oriUI, transform);
            T openUICom = newUI.GetComponent<T>();
            if (null == openUICom)
                return;
           

            openUICom.transform.SetAsLastSibling();
            listOpenUI.Add(openUICom);

            openUICom.Initialize();
            openUICom.Open();
        }
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

    public T GetUI<T>() where T : UI_Base
    {
        T openUI = listOpenUI.Find((UI_Base item) =>
        {
            if (item.GetType() == typeof(T))
                return true;
            return false;
        }) as T;

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
