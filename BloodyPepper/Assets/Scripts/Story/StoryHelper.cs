using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Story
{

    //스토리 관련 커맨드.
    public class StoryCommand
    {
        public enum CommandType
        {
              None
            , Select       // 현재 진행중인 스토리 스크립트에서 클릭 이벤트 발생            
        }

        public CommandType Command { get; private set; }
        public int Value { get; private set; }

        public StoryCommand(CommandType aCommand, int aValue)
        {
            Command = aCommand;
            Value = aValue;
        }
    }

    public static class StoryHelper
    {

    }
}

