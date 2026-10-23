using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class STutorialManager : MonoBehaviour
{ 
        public List<DialogueLine> dialogueLines;
        public DialogueLine repeatedInteractionLine;
        public UnityEvent OnInitialDialogueComplete;
        public bool hasTalked = false;


    [Serializable]
    public class DialogueLine
    {
        public string Text;
        public UnityEvent OnLineComplete;
    }
}