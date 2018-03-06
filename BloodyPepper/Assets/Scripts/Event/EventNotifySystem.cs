using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventNotifySystem
{
    public enum EVENT_ID : int
    {
        GOLD_CHANGE,    //금화 변경.
    }

    private EventNotifySystem() {}
    private static EventNotifySystem _instance = null;
    public static EventNotifySystem Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EventNotifySystem();

            return _instance;
        }
    }

    

    public delegate void EventNotifyCallback(Int64[] aEventValues);
    private Dictionary<EVENT_ID, EventNotifyCallback> dicEventNotifyCallbacks = new Dictionary<EVENT_ID, EventNotifyCallback>();

    //해당 EVENT가 발생시 실행할 함수 등록.
    public void AddEventCallback(EVENT_ID aEventID, EventNotifyCallback aCallback)
    {
        if (false == dicEventNotifyCallbacks.ContainsKey(aEventID))
            dicEventNotifyCallbacks.Add(aEventID, aCallback);
        else
            dicEventNotifyCallbacks[aEventID] += aCallback;
    }

    //해당 EVENT가 발생시 실행할 함수 제거.
    public void RemoveEventCallback(EVENT_ID aEventID, EventNotifyCallback aCallback)
    {
        if (dicEventNotifyCallbacks.ContainsKey(aEventID))
            dicEventNotifyCallbacks[aEventID] -= aCallback;
    }

    //EVENT 발생 알림.
    public void NotifyEvent(EVENT_ID aEventID, Int64[] aEventValues)
    {
        if (false == dicEventNotifyCallbacks.ContainsKey(aEventID))
            return;

        if (null == dicEventNotifyCallbacks[aEventID])
            return;

        dicEventNotifyCallbacks[aEventID](aEventValues);
    }
}
