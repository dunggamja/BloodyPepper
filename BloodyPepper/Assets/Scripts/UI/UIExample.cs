//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//코드 참조를 위해서 남겨놓음.
//public class UIExample: UI_Base
//{
//    public UI_GridController workGridController;
//    public UI_GridController trainGridController;

//    public Text goldText = null;


//    private void EventCallback_GoldChange(Int64[] aEventValues)
//    {
//        Int64 curGold = 0;
//        Int64 changeGoldAmount = 0;
//        if (2 <= aEventValues.Length)
//        {
//            curGold = aEventValues[0];
//            changeGoldAmount = aEventValues[1];
//        }

//        SetGoldText(curGold);
//    }

//    private void Awake()
//    {
//        EventNotifySystem.Instance.AddEventCallback(EventNotifySystem.EVENT_ID.GOLD_CHANGE, EventCallback_GoldChange);
//    }

//    private void OnDestroy()
//    {
//        EventNotifySystem.Instance.RemoveEventCallback(EventNotifySystem.EVENT_ID.GOLD_CHANGE, EventCallback_GoldChange);
//    }



//    public override void Open()
//    {
//        base.Open();

//        Initialize_WorkGrid();
//        Initialize_TrainGrid();

//        SetGoldText(GameMain.Instance.UserInfo.Gold);
//    }

//    private void Initialize_WorkGrid()
//    {
//        List<Work> workList = GameMain.Instance.WorkMgr.GetWorkList();

//        workGridController.ClearGridItem();
        
//        //메모리 할당
//        workGridController.listGridItemData.Capacity = workList.Count;

//        for (int i = 0; i < workList.Count; ++i)
//        {
//            workGridController.listGridItemData.Add(new UI_WorkGridData(workList[i].Kind));
//        }

//        workGridController.CreateGridItem();
//    }

//    private void Initialize_TrainGrid()
//    {
//        trainGridController.ClearGridItem();
//        trainGridController.CreateGridItem();
//    }

//    private void SetGoldText(Int64 aCurGold)
//    {
//        goldText.text = aCurGold.ToString("N0");
//    }
//}
