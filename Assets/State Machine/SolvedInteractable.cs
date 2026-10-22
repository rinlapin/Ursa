using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SolvedInteractable : Interactable
{
        public List<DialogueLine> dialogueLines;
        public DialogueLine repeatedInteractionLine;
        public UnityEvent OnInitialDialogueComplete;
        public bool hasTalked = false;

    public void OnDisable()
    {
        if (GameManager.Instance.ClosestInteractable == this)
        {
            GameManager.Instance.ClosestInteractable = null;
        }
    }
}

[Serializable]
public class DialogueLine
{
        public string Text;
        public UnityEvent OnLineComplete;
}
