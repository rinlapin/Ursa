using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SolvedInteractable : Interactable
{
        [field: SerializeField] public List<DialogueLine> DialogueLines { get; protected set; }
}

[Serializable]
public class DialogueLine
{
        public string text;
        public UnityEvent onComplete;
        public float timeToWait;
}
