using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Story;

public partial class DialogueUI
{
    //각 대화창 UI 처리에 대한 함수객체들.
    //DialogueUI 접근을 위해 inner class...
    public abstract class DUCommand
    {
        //public const uint COMMAND_SHOW     = 0x00000001; //등장
        //public const uint COMMAND_HIDE     = 0x00000002; //소멸
        //public const uint COMMAND_LIGHT    = 0x00000004; //밝게
        //public const uint COMMAND_GRAY     = 0x00000008; //회색처리.
        //public const uint COMMAND_SHAKE    = 0x00000010; //흔들림 처리.
        //public const uint COMMAND_ZOOM     = 0x00000020; //(이벤트 이미지용) 확대 처리.
        //public const uint COMMAND_IMMEDIATE= 0x10000000; //즉시 변경
        //public const uint COMMAND_INIT     = 0x20000000; //초기화.

        //public const uint TARGET_DIALOGUE_WINDOW            = 0x00000001;  //텍스트 창, 네임 태크, 텍스트포함.
        //public const uint TARGET_DIALOGUE_PORTRAIT_LEFT     = 0x00000002;  //왼쪽 초상화
        //public const uint TARGET_DIALOGUE_PORTRAIT_RIGHT    = 0x00000004;  //오른쪽 초상화
        //public const uint TARGET_DIALOGUE_EVENT_IMAGE       = 0x00000008;  //이벤트 이미지
        //public const uint TARGET_DIALOGUE_UNUSED_PORTRAIT = 0x00000010; //현재 사용안하고 있는 초상화.

        //public const uint TARGET_ALL                        = 0xFFFFFFFF;  //모두.

        //public uint     UICommand { get; private set; }
        //public uint     Target { get; private set; }

        //Value는 일단 4개까지.
        //public int Value1 { get; private set; }
        //public int Value2 { get; private set; }
        //public int Value3 { get; private set; }
        //public int Value4 { get; private set; }

        //public DialogueUI_Command(uint cmd, uint targetValue
        //    , int value1 = 0, int value2 = 0, int value3 = 0, int value4 = 0)
        //{
        //    UICommand = cmd;
        //    Target = targetValue;

        //    Value1 = value1;
        //    Value2 = value2;
        //    Value3 = value3;
        //    Value4 = value4;
        //}


        private bool _isComplete = false;
        public bool IsComplete { get { return _isComplete; } private set { _isComplete = value; } }

        public IEnumerator RunCommand()
        {
            IsComplete = false;
            yield return ProgressCommand();
            IsComplete = true;
        }

        protected abstract IEnumerator ProgressCommand();


        protected bool IsDialogueUIOpened()
        {
            var ui = UIManager.Instance.FindUI<DialogueUI>();
            return ui != null ? ui.IsVisible : false;
        }
    }

    public class DUCommand_Init : DUCommand
    {
        private readonly float INIT_TIME = 0.5f;
        private readonly float MOVE_VALUE = 100f;

        protected override IEnumerator ProgressCommand()
        {
            yield return null;
        }
    }

    public class DUCommand_Normal : DUCommand
    {
        PortraitInfo portInfo = null;
        string text = string.Empty;
        public DUCommand_Normal(PortraitInfo _portInfo, string _text)
        {
            portInfo = _portInfo;
            text = _text;
        }

        protected override IEnumerator ProgressCommand()
        {
            yield return null;
        }
    }



    public static void ShowDU(PortraitInfo[] portInfos)
    {

    }

}



