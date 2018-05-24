using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

//대사 스크립트 
public class DialogueScript : StoryScript
{
    public enum PORTRAIT_TYPE
    {
            NONE_PORTRAIT
        ,   ONE_PORTRAIT
        ,   TWO_PORTRAIT
    };

    private PORTRAIT_TYPE portraitType = PORTRAIT_TYPE.NONE_PORTRAIT;

    private string dialogue = string.Empty;
    private StringBuilder showingDialogue = new StringBuilder();

    private bool isCompleted = false;
    private float textPerSeconds = 5f;
    private Coroutine dialogueCoroutine = null;
    private Text labelDialogue = null;



    public DialogueScript(StoryTrigger triggerInfo, string dialogueText, Text label) 
        : base(triggerInfo)
    {
        dialogue = dialogueText;
        showingDialogue.Length = 0;
        showingDialogue.Capacity = dialogue.Length;
        labelDialogue = label;

        if (labelDialogue) labelDialogue.text = string.Empty;
    }

    public override void PlayStart()
    {
        dialogueCoroutine = GameMain.Instance.StartCoroutine(ShowDialogueSmoothly());
    }

    private IEnumerator ShowDialogueSmoothly()
    {
        while(false == IsCompleted)
        {
            
            AccumulatedTime += Time.deltaTime;
            int showTextLength = (int)(AccumulatedTime * textPerSeconds);

            SetDialogueTextLength(showTextLength);
            IsCompleted = CheckComplete();
            yield return null;
        }

        dialogueCoroutine = null;
        StoryManager.Instance.UpdatePlayingSequence();
    }

    private bool CheckComplete()
    {
        if (showingDialogue.Length < dialogue.Length)
            return false;

        return true;
    }

    private void SetDialogueTextLength(int length)
    {
        if (dialogue.Length <= showingDialogue.Length)
            return;

        length = Math.Min(dialogue.Length, length);

        if (length <= showingDialogue.Length)
            return;

        int subStringStart = showingDialogue.Length;
        int subStringLength = length - subStringStart;

        showingDialogue.Append(dialogue.Substring(subStringStart, subStringLength));
        if (labelDialogue) labelDialogue.text = showingDialogue.ToString();
    }
}
