using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SolvedInteractable : Interactable
{
        public List<DialogueLine> dialogueLines;
        public UnityEvent OnDialogueComplete;
}

[Serializable]
public class DialogueLine
{
        public string Text;
        public UnityEvent OnLineComplete;
}
