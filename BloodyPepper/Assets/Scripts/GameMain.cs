using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameMain : MonoBehaviour
{
    public static GameMain Instance { get; private set; }

    public delegate void UpdateTime(float aDeltaTime);
    public UpdateTime scaledTimeUpdate;         // 게임 시간에 맞춰서 갱신되야 하는 항목들은 이곳에  (ex: 유닛 생산, 퀘스트 제한 시간 등)
    public UpdateTime unscaledTimeUpdate;       // 게임 시간에 흐름에 상관없이 갱신되어야 하는 항목들은 이곳에 (ex: 플레이 시간 등)


    public delegate IEnumerator delegateCoroutine();
    public delegateCoroutine GameMainCoroutine;

    public GameObject[] DontDestroyOnLoadObjects; //신 로딩시 유지되어야 하는 오브젝트들을 등록합니다. 



    public StageManager StageMgr { get; private set; }


    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        if (null != DontDestroyOnLoadObjects)
        {
            for (int i = 0; i < DontDestroyOnLoadObjects.Length; ++i)
                DontDestroyOnLoad(DontDestroyOnLoadObjects[i]);
        }

        if (null == StageMgr)
            StageMgr = new StageManager();


        scaledTimeUpdate += StageMgr.Update;
    }


    public void Start()
    {
    }

    public void OnDestroy()
    {
        scaledTimeUpdate = null;


        StageMgr = null;
    }

    private void Update()
    { 
        if(null != scaledTimeUpdate)
            scaledTimeUpdate(UnityEngine.Time.deltaTime);           // 게임 시간에 맞춰서 갱신되야 하는 항목들은 이곳에  (ex: 유닛 생산, 퀘스트 제한 시간 등)
        if(null != unscaledTimeUpdate)
            unscaledTimeUpdate(UnityEngine.Time.unscaledDeltaTime); // 게임 시간에 흐름에 상관없이 갱신되어야 하는 항목들은 이곳에 (ex: 플레이 시간 등)
    }
}
