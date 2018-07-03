using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Story
{
    public enum CharacterType
    {
        None
        , Asuka
    };

    public enum EmotionType
    {
        Normal
        , Angry
        , Sad
    };

    public enum PortraitPosition
    {
        None
        , Left
        , Right
    };


    public class PortraitInfo
    {
        public PortraitPosition portraitType = PortraitPosition.None;
        public CharacterType characterType = CharacterType.None;
        public EmotionType emotionType = EmotionType.Normal;

        public PortraitInfo(PortraitPosition portrait, CharacterType character, EmotionType emotion)
        {
            portraitType = portrait;
            characterType = character;
            emotionType = emotion;
        }
    };
};

public class DialogueUI_Command
{
    public const uint COMMAND_SHOW     = 0x00000001; //등장
    public const uint COMMAND_HIDE     = 0x00000002; //소멸
    public const uint COMMAND_LIGHT    = 0x00000004; //밝게
    public const uint COMMAND_GRAY     = 0x00000008; //회색처리.
    public const uint COMMAND_SHAKE    = 0x00000010; //흔들림 처리.
    public const uint COMMAND_ZOOM     = 0x00000020; //(이벤트 이미지용) 확대 처리.
    public const uint COMMAND_IMMEDIATE= 0x10000000; //즉시 변경

    public const uint TARGET_DIALOGUE_WINDOW            = 0x00000001;  //텍스트 창, 네임 태크, 텍스트포함.
    public const uint TARGET_DIALOGUE_PORTRAIT_LEFT     = 0x00000002;  //왼쪽 초상화
    public const uint TARGET_DIALOGUE_PORTRAIT_RIGHT    = 0x00000004;  //오른쪽 초상화
    public const uint TARGET_DIALOGUE_EVENT_IMAGE       = 0x00000008;  //이벤트 이미지

    public const uint TARGET_ALL                        = 0xFFFFFFFF;  //모두.
    
    public uint     UICommand { get; private set; }
    public uint     Target { get; private set; }

    //Value는 일단 4개까지.
    public int Value1 { get; private set; }
    public int Value2 { get; private set; }
    public int Value3 { get; private set; }
    public int Value4 { get; private set; }

    public DialogueUI_Command(uint cmd, uint targetValue
        , int value1 = 0, int value2 = 0, int value3 = 0, int value4 = 0)
    {
        UICommand = cmd;
        Target = targetValue;

        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
    }
}



public class DialogueUI : UI_Base
{
    [SerializeField] private GameObject dialogueObj = null;
    [SerializeField] private Text dialogueText = null;
    [SerializeField] private GameObject nameObj = null;
    [SerializeField] private Text nameText = null;
    [SerializeField] private RectTransform namePosL = null;
    [SerializeField] private RectTransform namePosR = null;
    [SerializeField] private Image portraitL = null;
    [SerializeField] private Image portraitR = null;
    [SerializeField] private Image backImage = null;


    private Vector3 portraitLeftPos = Vector3.zero;
    private Vector3 portraitRightPos = Vector3.zero;

    public enum IMAGE_TYPE
    {
          PORTRAIT_LEFT      //왼쪽 초상화
        , PORTRAIT_RIGHT     //오른쪽 초상화
        , BACKIMAGE          //이벤트 씬용 이미지. 
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Initialize()
    {
        base.Initialize();
        portraitLeftPos = portraitL.rectTransform.localPosition;
        portraitRightPos = portraitR.rectTransform.localPosition;
    }


    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    private void SetImage(IMAGE_TYPE type, Sprite sprite)
    {
        Image image = GetImageObject(type);
        if (null == image)
            return;

        if (image.sprite == sprite)
            return;

        image.sprite = sprite;
    }

    private void SetEnable(IMAGE_TYPE type, bool enable)
    {
        Image image = GetImageObject(type);
        if (null == image)
            return;

        image.gameObject.SetActive(enable);
    }

    private Image GetImageObject(IMAGE_TYPE type)
    {
        switch (type)
        {
            case IMAGE_TYPE.BACKIMAGE:
                return backImage;
            case IMAGE_TYPE.PORTRAIT_LEFT:
                return portraitL;
            case IMAGE_TYPE.PORTRAIT_RIGHT:
                return portraitR;
        }
        return null;
    }


    public void SetImageSmooth(IMAGE_TYPE type, Sprite sprite, float playTime = 1f)
    {
        Image image = GetImageObject(type);
        if (null == image)
            return;

        if (image.sprite == sprite)
            return;

        switch (type)
        {
            case IMAGE_TYPE.BACKIMAGE:
                {
                    DG.Tweening.DOTween.To(() => image.color, x => image.color = x, Color.black, playTime * 0.5f)
                        .onComplete = () =>
                        {
                            image.sprite = sprite;
                            DG.Tweening.DOTween.To(() => image.color, x => image.color = x, Color.white, playTime * 0.5f);
                        };
                }
                break;
            case IMAGE_TYPE.PORTRAIT_LEFT:
            case IMAGE_TYPE.PORTRAIT_RIGHT:
                {
                    Vector3 originPos = Vector3.zero;
                    Vector3 targetPos = Vector3.zero;
                    float movePosX = 200f;

                    if (type == IMAGE_TYPE.PORTRAIT_LEFT)
                    {
                        targetPos = portraitLeftPos + new Vector3(-movePosX, 0f);
                        originPos = portraitLeftPos;
                    }
                    else
                    {
                        targetPos = portraitRightPos + new Vector3(movePosX, 0f);
                        originPos = portraitRightPos;
                    }

                    DG.Tweening.DOTween.To(() => image.rectTransform.localPosition, x => image.rectTransform.localPosition = x, targetPos, playTime * 0.5f)
                        .onComplete = () =>
                        {
                            image.sprite = sprite;
                            DG.Tweening.DOTween.To(() => image.rectTransform.localPosition, x => image.rectTransform.localPosition = x, originPos, playTime * 0.5f);
                        };
                }
                break;
        }
    }

    public void SetDialogueText(string name, string text)
    {
        if(false == nameText.Equals(name))
        {

        }
    }

    //현재 대화창 UI 상태에 따라서 UI 행동들을 만들어 냅니다. 
    // 현재 UI 창이 열려있는가 ? 
    //     : 열려있지 않다면 UI창을 등장시킨다. (초상화와 이름도 같이 등장시킨다.)
    //     : 열려있다면 이번에 대사를 하는 캐릭터와 초상화 정보를 체크한다. 
    //            : 같은 캐릭터가 이어서 대사를 한다. : 대사만 진행시킨다. 
    //            : 다른 캐릭터가 이어서 대사를 한다. : 이전의 대사를 치던 캐릭터 초상화를 어둡게 처리. 
    //                 다른캐릭터가 이미 대사창에 초상화가 표시되어 있었으면 밝게 처리, 아닐 경우 등장처리.   (이름 태그의 위치를 바꿔준다.)
    //                            : 등장위치에 다른 캐릭터 초상화가 있었으면 사라짐 처리 후 등장처리.         (이름 태그의 위치를 바꿔준다.)
    //
    // 재생이 끝났는데. 다음 대화창에 표시할 내용이 없다면? 대화창을 닫아줘야 하는데... 
    //   : 그 판단을 자동으로 하긴 힘들다.. 대화창 닫기는 따로 처리한다...
    public List<DialogueUI_Command[]> MakeUICommandByScript(Story.DialogueScript dialogueScript)
    {
        List<DialogueUI_Command[]> listActions = new List<DialogueUI_Command[]>();

        if(false == Initialized)
        {
            //UI가 현재 보이지 않는 상태라면 등장!! 
            listActions.Add(new DialogueUI_Command[]
                {
                    new DialogueUI_Command(DialogueUI_Command.COMMAND_SHOW, DialogueUI_Command.TARGET_ALL) 
                });
        }

        return listActions;
    }

    public IEnumerator PlayUICommand(DialogueUI_Command[] commands)
    {
        yield return null;
    }

}
