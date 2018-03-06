//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//코드 참조를 위해서 남겨놓음.
//public class UI_WorkGridData : UI_GridItemData
//{
//    public int workKind = 0;

//    public UI_WorkGridData(int aKind)
//    {
//        workKind = aKind;
//    }
//}

//public class UI_WorkGrid : UI_GridItem
//{
//    public Slider sliderWorkProgress ;
//    public Text textWorkDescription ;

//    public Button buttonUpgrade10;
//    public Button buttonUpgrade1;

//    public Text textUpgrade10;
//    public Text textUpgrade1;

//    private int workKind = 0;

//    private void Awake()
//    {
//        EventNotifySystem.Instance.AddEventCallback(EventNotifySystem.EVENT_ID.GOLD_CHANGE, EventCallback_GoldChange);
//        EventNotifySystem.Instance.AddEventCallback(EventNotifySystem.EVENT_ID.WORK_LEVELUP, EventCallback_WorkLevelup);
//    }

//    private void OnDestroy()
//    {
//        EventNotifySystem.Instance.RemoveEventCallback(EventNotifySystem.EVENT_ID.GOLD_CHANGE, EventCallback_GoldChange);
//        EventNotifySystem.Instance.RemoveEventCallback(EventNotifySystem.EVENT_ID.WORK_LEVELUP, EventCallback_WorkLevelup);
//    }

//    private void EventCallback_GoldChange( Int64[] aEventValues)
//    {
//        SetButtons(GameMain.Instance.WorkMgr.GetWorkInfo(workKind));
//    }

//    private void EventCallback_WorkLevelup(Int64[] aEventValues)
//    {
//        var curWork = GameMain.Instance.WorkMgr.GetWorkInfo(workKind);
//        if(null == curWork)
//            return;
//        SetText(curWork);
//        SetSliderValue(curWork);
//        SetButtons(curWork);
//    }

//    public override void Initialize()
//    {
//        base.Initialize();

//        Button.ButtonClickedEvent clickEvent = new Button.ButtonClickedEvent();
//        clickEvent.AddListener(() => { ClickUpgradeButton(1); });
//        buttonUpgrade1.onClick = clickEvent;


//        Button.ButtonClickedEvent clickEvent2 = new Button.ButtonClickedEvent();
//        clickEvent2.AddListener(() => { ClickUpgradeButton(10); });
//        buttonUpgrade10.onClick = clickEvent2;
//    }


//    private void InitData()
//    {
//        workKind = 0;
//    }
    
//    public override void UpdateUI(UI_GridItemData itemData)
//    {
//        base.UpdateUI(itemData);

//        InitData();        

//        UI_WorkGridData workData = itemData as UI_WorkGridData;
//        if (null == workData)
//            return;

//        workKind = workData.workKind;
//        if (0 == workKind)
//            return;

//        Work curWork = GameMain.Instance.WorkMgr.GetWorkInfo(workKind);

//        SetText(curWork);
//        SetSliderValue(curWork);
//        SetButtons(curWork);
//    }

//    private void SetText(Work aWorkInfo)
//    {
//        if (null == aWorkInfo)
//            return;

//        textWorkDescription.text = string.Format("레벨 : {0}, 버는 돈 : {1}", aWorkInfo.Level, aWorkInfo.GetEarningGold());
//    }

//    private void SetSliderValue(Work aWorkInfo)
//    {
//        if (null == aWorkInfo)
//            return;

//        int totalTime = aWorkInfo.WorkTime;
//        if (totalTime <= 0)
//            totalTime = 1;

//        sliderWorkProgress.value = Mathf.Clamp01((float)aWorkInfo.CurAccumulateTime / (float)totalTime);
//    }

//    private void SetButtons(Work aWorkInfo)
//    {
//        if (null == aWorkInfo)
//            return;

//        GAMEDATA.DATA.GAMEDATA_WORK workData = GAMEDATA.GAMEDATAINFOS.Instance.GetWorkData(aWorkInfo.Kind);
//        if (null == workData)
//            return;

//        if (workData.maxLevel <= aWorkInfo.Level)
//        {
//            buttonUpgrade1.gameObject.SetActive(false);
//            buttonUpgrade10.gameObject.SetActive(false);
//            return;
//        }

//        if (workData.maxLevel < aWorkInfo.Level + 10)
//            buttonUpgrade10.gameObject.SetActive(false);
//        else
//            buttonUpgrade10.gameObject.SetActive(true);


//        int next10Level = Math.Min(workData.maxLevel, aWorkInfo.Level + 10);

//        int nextRequireGold = 0;
//        int next10RequireGold = 0;

//        for (int i = aWorkInfo.Level; i < next10Level; ++i)
//        {
//            var workLevelData = GAMEDATA.GAMEDATAINFOS.Instance.GetWorkLevelData(aWorkInfo.Kind, i);

//            if (null == workLevelData)
//                continue;

//            if (i == aWorkInfo.Level)
//            {
//                nextRequireGold = workLevelData.RequireForNextLevel;
//            }

//            next10RequireGold += workLevelData.RequireForNextLevel;
//        }


//        textUpgrade1.text = nextRequireGold.ToString("N0");
//        textUpgrade10.text = next10RequireGold.ToString("N0");
//    }

//    private void Update()
//    {
//        if (0 == workKind)
//            return;

//        SetSliderValue(GameMain.Instance.WorkMgr.GetWorkInfo(workKind));
//    }

//    private void ClickUpgradeButton(int aUpgradeLevel)
//    {
//        var curWorkInfo = GameMain.Instance.WorkMgr.GetWorkInfo(workKind);

//        GAMEDATA.DATA.GAMEDATA_WORK workData = GAMEDATA.GAMEDATAINFOS.Instance.GetWorkData(curWorkInfo.Kind);
//        if (null == workData)
//            return;

//        int targetLevel = curWorkInfo.Level + aUpgradeLevel;

//        if (workData.maxLevel < targetLevel)
//        {
//            return;
//        }


//        Int64 requireGold = 0;
//        for(int i = curWorkInfo.Level; i < targetLevel; ++i)
//        {
//            var workLevelData = GAMEDATA.GAMEDATAINFOS.Instance.GetWorkLevelData(curWorkInfo.Kind, i);
//            if (null == workLevelData)
//                continue;

//            requireGold += workLevelData.RequireForNextLevel;
//        }

//        if(GameMain.Instance.UserInfo.SubstructGold(requireGold))
//            GameMain.Instance.WorkMgr.SetWork(curWorkInfo.Kind, targetLevel);
//    }
//}
