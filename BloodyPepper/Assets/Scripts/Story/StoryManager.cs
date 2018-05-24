﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//트리거 클래스.
public struct StoryTrigger
{
    public enum SEQUANTIAL_TYPE
    {
        Auto,       //실행중인 스크립트들이 완료되면 자동으로 실행된다.   (이전 스크립트가 없으면 자동실행)
        Manually,   //따로 설정해둔 트리거 키가 ON 되어야 실행된다.       (Manually이고 트리거 키가 없으면 무조건으로 실행)
    }

    public  SEQUANTIAL_TYPE sequaltialType  { get; private set; }
    public  string[] triggerKeys { get; private set; }

    public StoryTrigger(SEQUANTIAL_TYPE type, string[] triggers)
    {
        sequaltialType = type;
        triggerKeys = triggers;
    }


    public bool IsFlaged(Dictionary<string, bool> flags)
    {
        if (null == triggerKeys)
            return true;

        foreach(var key in triggerKeys)
        {
            if (flags.ContainsKey(key) == false)
                return false;

            if (flags[key] == false)
                return false;
        }

        return true;
    }
}

//스토리 스크립트
// (대사/서술/지문/선택지) 등을 구현하자.
public abstract class StoryScript
{
    public StoryScript(StoryTrigger triggerInfo)
    {
        RequiredTrigger = triggerInfo;             // 요구되는 트리거 목록 초기화
        AccumulatedTime = 0f;
        IsCompleted = false;
    }

    
    // 이 데이터들은 생성할때 꼭 처리되어야 한다. !! 
    //  트리거 목록도 생성할때 처리하도록 하자.
    public StoryTrigger RequiredTrigger { get; private set; }   // 이 스크립트가 실행될 조건 트리거.
    public float AccumulatedTime { get; protected set; }        // 누적 시간값.
    public bool IsCompleted { get; protected set; }             // 재생이 완료되었는가?


    // 스크립트를 재생한다. 
    //  : 재생도중에 트리거들을 반환한다. 
    //  : trigger
    public abstract void PlayStart();

}


//연속된 스크립트를 재생한다.  
// : 1개의 시퀸스에 들어가있는 내용은 시작하면 무조건 재생이 된다.  
// : 선택지의 경우 시퀸스가 나눠지게 된다. 
//  예)  1번 시퀸스에서 선택문 a, b가 나옴 
//        a 선택 -> 2번 시퀸스로,   b 선택 -> 3번 시퀸스로.
// : 매 프레임마다 실행이 된다라고 일단 가정을 하고 있다. 
public class StorySequence
{
    public StorySequence(Int32 sequenceNum, StoryScript[] scripts)
    {
        SequenceNum = sequenceNum;

        foreach(var scriptInfo in scripts)
        {
            scriptsInWaiting.Enqueue(scriptInfo);
        }
    }

    //시퀸스 넘버.
    public Int32 SequenceNum { get; private set; }       

    //트리거 항목들.
    //시퀸스가 트리거 항목에 대한 정보를 들고 있는 것이 좋을 것 같다. 
    //각 스크립트가 실행되면서 trigger에 대한 요청을 줄 것이다.
    private Dictionary<string, bool> triggerFlags = new Dictionary<string, bool>();   


    //대기중인 스토리 이벤트는 순차적으로 처리가 될것이다. 큐형태로 관리 된다.
    //선행 스토리 이벤트가 먼저 재생되야 다음 항목이 재생될수 있다.
    //스토리 이벤트는 여러개가 동시에 재생될수 있다. 
    // 예) 대사를 하면서 걸어가는 캐릭터
    private Queue<StoryScript> scriptsInWaiting = new Queue<StoryScript>();    

    //현재 재생중인 스토리 이벤트 항목들을 관리한다. 
    //재생이 완료된 항목들은 제거된다.
    private List<StoryScript> scriptsInPlaying = new List<StoryScript>();    
   
    public void SetTrigger(string[] triggers)
    {
        if(null != triggers)
        {
            foreach(var key in triggers)
            {
                if (string.IsNullOrEmpty(key) == false)
                    triggerFlags[key] = true;
            }
        }
    }

    public void UpdateSequence()
    {
        //현재 재생 완료된 스크립트 제거 -> 대기중 스크립트 재생시작
        //로직 순서를 바꾸면 안된다. 

        RemovePlayingScriptsInCompleted();  //재생상태인 애들중 완료된 애들 컨테이너에서 제거!!
        PlayingScriptsInWaiting();         //대기상태인 애들중 재생가능한 애들 실행!!

        //재생이 완료되었는가??
        if (IsCompleted())
            StoryManager.Instance.OnCurrentSequenceComplete();
    }



    //대기상태인 스크립트중 재생가능한 상태인 것들을 옮긴다.
    private void PlayingScriptsInWaiting()
    {
        //트리거 체크.
        Func<StoryScript, bool> IsTriggeredScript = (StoryScript script) => 
        {
            //널 스크립트일 경우에도 true 처리.
            if (null == script)
                return true;

            //트리거 정보 검사.
            var triggerInfo = script.RequiredTrigger;

            if (StoryTrigger.SEQUANTIAL_TYPE.Manually == triggerInfo.sequaltialType)
            {
                //트리거 목록 검사.
                return triggerInfo.IsFlaged(triggerFlags);
            }
            else if (StoryTrigger.SEQUANTIAL_TYPE.Auto == triggerInfo.sequaltialType)
            {
                //현재 실행중인 스크립트가 없으면 실행!!
                return scriptsInPlaying.Count == 0;
            }

            return false;
        };

        
        while(0 < scriptsInWaiting.Count)
        {
            var scriptInfo = scriptsInWaiting.Peek();

            if(IsTriggeredScript(scriptInfo))
            {
                var playScript = scriptsInWaiting.Dequeue();
                if(null != playScript)
                {
                    playScript.PlayStart();
                    scriptsInPlaying.Add(playScript);
                }
            }
            else
            {
                break;
            }
        }
    }

    private void RemovePlayingScriptsInCompleted()
    {
        scriptsInPlaying.RemoveAll((StoryScript scripts) => 
        {
            if (null == scripts)
                return true;

            return scripts.IsCompleted;
        });
    }

    private void CheckScriptAndNotifiy()
    {
        StoryManager.Instance.OnCurrentSequenceComplete();
    }

    public bool IsCompleted()
    {
        if (0 < scriptsInWaiting.Count)
            return false;

        if (0 < scriptsInPlaying.Count)
            return false;

        return true;
    }
}


public class StoryManager
{
#region  SingletonPattern 
    private StoryManager() { }
    private static StoryManager _instance = null;
    public static StoryManager Instance
    {
        get
        {
            if (null == _instance)
                _instance = new StoryManager();
            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }
    #endregion


#region SequenceContainers

    // 메모리 상에 들고 있는 시퀸스들 
    private Dictionary<Int32, StorySequence>    sequenceRepository = new Dictionary<int, StorySequence>();

    // 현재 실행 (or 실행 대기) 중인 시퀸스 큐. 
    private Queue<StorySequence>                playingSequenceQueue = new Queue<StorySequence>();

#endregion


    //현재 실행중인 시퀸스에 트리거 변경사항을 적용한다.!!
    public void SetTriggers(string[] triggers)
    {
        if (0 < playingSequenceQueue.Count)
        {
            var currentSequence = playingSequenceQueue.Peek();
            if(null != currentSequence)
            {
                currentSequence.SetTrigger(triggers);
            }
        }
    }

    //현재 실행중인 시퀸스 변경사항 적용
    public void UpdatePlayingSequence()
    {
        if (0 < playingSequenceQueue.Count)
        {
            var currentSequence = playingSequenceQueue.Peek();
            if (null != currentSequence)
            {
                currentSequence.UpdateSequence();
            }
        }
    }


    public void AddSequenceInfo(StorySequence sequence)
    {
        if (null == sequence)
            return;

        sequenceRepository[sequence.SequenceNum] = sequence;
    }

    // 로드된 시퀸스를 실행(or 실행대기) 상태로 만든다. 
    public void EnqueuePlayingSequenceFromLoadedSequences(int sequenceNum)
    {
        if(sequenceRepository.ContainsKey(sequenceNum))
        {
            playingSequenceQueue.Enqueue(sequenceRepository[sequenceNum]);

            //실행중인 시퀸스가 없었을 경우 갱신!!
            if(1 == playingSequenceQueue.Count)
                UpdatePlayingSequence();
        }
    }

    public void OnCurrentSequenceComplete()
    {
        if (0 < playingSequenceQueue.Count)
        {
            var currentSequence = playingSequenceQueue.Peek();

            //현재 시퀸스가 끝났거나 널인지 한번더 체크해준다.
            if (null == currentSequence
            || currentSequence.IsCompleted())
            {
                playingSequenceQueue.Dequeue();
                UpdatePlayingSequence();
            }
        }
    }


#region clearMethods
    public void ClearLoadedSequences()
    {
        sequenceRepository.Clear();
    }

    public void ClearPlayingSequences()
    {
        //현재 진행중인 시퀀스에 대한 중단처리도 필요한가?? 
        playingSequenceQueue.Clear();
    }

    public void Clear()
    {
        ClearLoadedSequences();
        ClearPlayingSequences();
    }
#endregion
}


public static class StoryTest
{
    public static void AddTestScript(UnityEngine.UI.Text labelObj)
    {
        StoryManager.Instance.AddSequenceInfo(
            new StorySequence( 1
            , new StoryScript[] 
            {
                new DialogueScript(new StoryTrigger(StoryTrigger.SEQUANTIAL_TYPE.Auto, null), "rerefefefesfsfsfsffsfdsfsfsfsf", labelObj),
                new DialogueScript(new StoryTrigger(StoryTrigger.SEQUANTIAL_TYPE.Auto, null),  "fdfdfdv\ndvdvddvvdvd", labelObj)
            }
            ));

        StoryManager.Instance.AddSequenceInfo(
           new StorySequence(2
           , new StoryScript[]
           {
                new DialogueScript(new StoryTrigger(StoryTrigger.SEQUANTIAL_TYPE.Auto, null), "1212121221\n3313", labelObj),
                new DialogueScript(new StoryTrigger(StoryTrigger.SEQUANTIAL_TYPE.Auto, null),  "46544646", labelObj)
           }
           ));

        StoryManager.Instance.EnqueuePlayingSequenceFromLoadedSequences(2);
        StoryManager.Instance.EnqueuePlayingSequenceFromLoadedSequences(1);
    }
}