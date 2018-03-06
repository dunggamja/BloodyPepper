using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager
{
    public Stage currentStage { get; private set; }

    public Action Post_OnCompleteSceneLoaded;

    public StageManager()
    {
        //씬 로드가 완료되면 실행된다. 
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        currentStage = null;
    }

    //Scene이 로드 완료 되었을때 실행되는 함수. 
    private void OnSceneLoaded(Scene aLoadedScene, LoadSceneMode aSceneMode)
    {
        //if (string.Equals(aLoadedScene.name, "GameScene"))
        //{
        //    UIManager.Instance.CloseAllUI();
        //    if(null != stageInfo)
        //        SetStage(stageInfo.Level);
        //    UIManager.Instance.OpenUI<MainUI>();
        //}

        if (null != Post_OnCompleteSceneLoaded)
            Post_OnCompleteSceneLoaded();
    }

    private bool SetStage(int aLevel)
    {
        //셋팅은 여기서 하고.... 
        currentStage = new Stage();
        //currentStage.Instantiate_Stage();
        return true;
    }

    public void Update(float aDeltaTime)
    {
        if (null != currentStage)
            currentStage.Update(aDeltaTime);
    }

    public void LoadLevel(int aLevel)
    {
        
        //if (false == string.Equals("GameScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name))
        //{
        //    GameMain.Instance.GameMainCoroutine += test;
        //    //현재 GameScene이 아닐 경우  씬로드 후 스테이지 오브젝트 생성.
        //    UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");

        //    stageInfo = new STAGEINFO_FOR_SAVE();
        //    stageInfo.Level = aLevel;
        //}
        //else
        //{
        //    //현재 GameScene일 경우 바로 스테이지 오브젝트 생성.
        //    SetStage(aLevel);
        //}
    }
}
