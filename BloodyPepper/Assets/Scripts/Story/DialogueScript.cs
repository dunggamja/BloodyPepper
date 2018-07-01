using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

namespace Story
{
    //대사 스크립트 
    public class DialogueScript : StoryScript
    {

        private PortraitInfo    portraitInfo = null;
        private string          originalMessage = string.Empty;
        private StringBuilder   showingDialogue = new StringBuilder();

        private float textPerSeconds = 5f;


        static public DialogueScript Create_DialogueEnd()
        {
            // 빈 문자열이 올경우 대화창을 닫는 거라고 생각하자. 
            // 대화창을 자동으로 닫기엔 판단 기준이 명확하지 않은 것 같다... 
            return new DialogueScript(string.Empty);
        }

        public DialogueScript(string message)
            : base(TriggerType.Auto, null)
        {
            originalMessage = message;
            showingDialogue.Length = 0;
            showingDialogue.Capacity = originalMessage.Length;
        }

        public DialogueScript(PortraitPosition portrait, CharacterType character, EmotionType emotion, string message)
            : this(message)
        {
            portraitInfo = new PortraitInfo(portrait, character, emotion);            
        }


        public override void NotifyCommand(StoryCommand command)
        {
            
        }

        protected override IEnumerator RunScript()
        {
            var listActions = DialogueUI.MakeUIActionsByScript(this);

            for(int i = 0; i < listActions.Count; ++i)
            {
                yield return listActions[i];
            }

            IsCompleted = true;
        }
       
        private void SetDialogueTextLength(int length)
        {
            if (originalMessage.Length <= showingDialogue.Length)
                return;

            length = Math.Min(originalMessage.Length, length);

            if (length <= showingDialogue.Length)
                return;

            int subStringStart = showingDialogue.Length;
            int subStringLength = length - subStringStart;

            showingDialogue.Append(originalMessage.Substring(subStringStart, subStringLength));
            //if (labelDialogue) labelDialogue.text = showingDialogue.ToString();
        }
    }
}