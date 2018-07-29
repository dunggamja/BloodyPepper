using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Story;


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
        , Hide      //표시하지 않는다. 
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


        public bool IsEqual(PortraitInfo info)
        {
            if(null == info)
                return false;

            return info.portraitType == portraitType && info.characterType == characterType && info.emotionType == emotionType;
        }
    };

    
};




public partial class DialogueUI : UI_Base
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


    private Vector3 originLeftPortraitPos = Vector3.zero;
    private Vector3 originRightPortraitPos = Vector3.zero;
    
    private PortraitInfo[] portraintInfos = new PortraitInfo[2] 
    {
          new PortraitInfo(PortraitPosition.Left, CharacterType.None, EmotionType.Normal) 
        , new PortraitInfo(PortraitPosition.Right, CharacterType.None, EmotionType.Normal)
    };


    //UI 오브젝트들을 구분하기 위한 enum값.
    public enum UIObjects
    {
          WINDOW
        , TEXT
        , NAME
        , NAME_TEXT
        , NAME_POS_L
        , NAME_POS_R
        , PORTRAIT_L
        , PORTRAIT_R
        , FULL_IMAGE
    }


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
        originLeftPortraitPos = portraitL.rectTransform.localPosition;
        originRightPortraitPos = portraitR.rectTransform.localPosition;
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
                        targetPos = originLeftPortraitPos + new Vector3(-movePosX, 0f);
                        originPos = originLeftPortraitPos;
                    }
                    else
                    {
                        targetPos = originRightPortraitPos + new Vector3(movePosX, 0f);
                        originPos = originRightPortraitPos;
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
    //     : 열려있지 않다면 UI창을 등장시킨다. (초상화와 이름도 같이 등장시킨다.) //요부분은 각 UI 커맨드가 알아서 해야 한다. 
    //     : 열려있다면 이번에 대사를 하는 캐릭터와 초상화 정보를 체크한다.        //요부분은 각 UI 커맨드가 알아서 해야 한다.
    //            : 같은 캐릭터가 이어서 대사를 한다. : 대사만 진행시킨다.        //요부분은 각 UI 커맨드가 알아서 해야 한다.
    //            : 다른 캐릭터가 이어서 대사를 한다. : 이전의 대사를 치던 캐릭터 초상화를 어둡게 처리.   //요부분은 각 UI 커맨드가 알아서 해야 한다.
    //                 다른캐릭터가 이미 대사창에 초상화가 표시되어 있었으면 밝게 처리, 아닐 경우 등장처리.   (이름 태그의 위치를 바꿔준다.)    //요부분은 각 UI 커맨드가 알아서 해야 한다.
    //                            : 등장위치에 다른 캐릭터 초상화가 있었으면 사라짐 처리 후 등장처리.         (이름 태그의 위치를 바꿔준다.)    //요부분은 각 UI 커맨드가 알아서 해야 한다.
    //
    // 재생이 끝났는데. 다음 대화창에 표시할 내용이 없다면? 대화창을 닫아줘야 하는데...  //요부분은 대화창 닫기 커맨드가 따로 있다. 
    //   : 그 판단을 자동으로 하긴 힘들다.. 대화창 닫기는 따로 처리한다...

    // 생각해보니.... 별로 MakeDUCommands가 할게 없네;;;;
    // : 대화창 UI의 상태는 각 행동이 실행될때 체크가 되어야하므로......;;;;;;;
    public List<DUCommand[]> MakeDUCommands(Story.DialogueScript dialogueScript)
    {
        List<DUCommand[]> listActions = new List<DUCommand[]>(16);
        


        return listActions;
    }

    public IEnumerator PlayDUCommands(DUCommand[] commands)
    {
        for (int i = 0; i < commands.Length; ++i)
            StartCoroutine(commands[i].RunCommand());

        yield return null;

        while (true)
        {
            bool isComplete = true;
            for (int i = 0; i < commands.Length; ++i)
                isComplete &= commands[i].IsComplete;

            if (isComplete)
                break;

            yield return null;
        }

    }
}
