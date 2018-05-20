using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//스토리 스크립트
// (대사/서술/지문/선택지) 등을 구현하자.
public abstract class StoryScript 
{
    public StoryScript(Int32 scriptID, string[] triggerNames)
    {
        ID = scriptID;                              //ID 설정
        RequiredTrigger = triggerNames;             //트리거 목록 초기화
    }

    
    // 이 데이터들은 생성할때 꼭 처리되어야 한다. !! 
    //  트리거 목록도 생성할때 처리하도록 하자.
    public Int32 ID { get; private set; }                   //ID
    public string[] RequiredTrigger { get; private set; }   //이 스크립트가 실행될 조건 트리거.


    public abstract bool        IsCompleted();    // 재생이 완료되었는가?
    
    public virtual void        Play(Action<string[]> callback)    // 스크립트를 재생을 시작한다.
    {
        funcTriggerSet = callback;
    }


    private Action              onComplete      = null;
    private Action<string[]>    funcTriggerSet  = null;    
}


//연속된 스크립트를 재생한다.  
// : 1개의 시퀸스에 들어가있는 내용은 시작하면 무조건 재생이 된다.  
// : 선택지의 경우 시퀸스가 나눠지게 된다. 
//  예)  1번 시퀸스에서 선택문 a, b가 나옴 
//        a 선택 -> 2번 시퀸스로,   b 선택 -> 3번 시퀸스로.
// : 매 프레임마다 실행이 된다라고 일단 가정을 하고 있다. 
public class StorySequence
{
    public Int64 ID { get; private set; }       //시퀸스 넘버.

    private Dictionary<string, bool> triggerInfos = new Dictionary<string, bool>();  //트리거 항목들.
                                                                                     //시퀸스가 트리거 항목에 대한 정보를 들고 있는 것이 좋을 것 같다. 
                                                                                     //각 스크립트가 실행되면서 trigger에 대한 요청을 줄 것이다. 
                                                                                     


    private Queue<StoryScript> waitStoryEvents = new Queue<StoryScript>();    //대기중인 스토리 이벤트는 순차적으로 처리가 될것이다. 큐형태로 관리 된다.
                                                                              //선행 스토리 이벤트가 먼저 재생되야 다음 항목이 재생될수 있다.
                                                                              //스토리 이벤트는 여러개가 동시에 재생될수 있다. 
                                                                              // 예) 대사를 하면서 걸어가는 캐릭터

    private List<StoryScript> playingStoryEvents = new List<StoryScript>();   //현재 재생중인 스토리 이벤트 항목들을 관리한다. 
                                                                              //재생이 완료된 항목들은 제거된다. 

    public delegate void TriggerCallback(string[] triggers);    
    
    public void UpdateTriggers(string[] triggers)
    {
        foreach (string key in triggers)
        {
            if (false == string.IsNullOrEmpty(key))
                triggerInfos[key] = true;
        }

        RunSequence();
    }

    // 시퀸스 재생!! 
    public void RunSequence()
    {
        PlayScriptInWaiting();
    }



    //대기상태인 스크립트중 재생가능한 상태인 것들을 재생해줍니다.. 
    private void PlayScriptInWaiting()
    {
        //트리거 체크.
        Func<StoryScript, bool> IsTriggeredScript = (StoryScript script) => 
        {
            //널 스크립트일 경우에도 true 처리.
            if (null == script)
                return true;

            //트리거 정보 검사.
            foreach(var key in script.RequiredTrigger)
            {
                if (string.IsNullOrEmpty(key))
                    continue;

                if (false == triggerInfos.ContainsKey(key))
                    return false;

                if (false == triggerInfos[key])
                    return false;
            }

            return true;
        };

        
        while(0 < waitStoryEvents.Count)
        {
            var scriptInfo = waitStoryEvents.Peek();

            if(IsTriggeredScript(scriptInfo))
            {
                var playScript = waitStoryEvents.Dequeue();
                if(null != playScript)
                {
                    //스크립트 재생 시작! 
                    playScript.Play(UpdateTriggers);
                    playingStoryEvents.Add(playScript);
                }
            }
            else
            {
                break;
            }
        }
    }

}
