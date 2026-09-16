using System;
using System.Collections.Generic;
using UnityEngine;

public class SolvedInteractable : Interactable
{
        [field: SerializeField] public List<string> DialogueLines { get; protected set; }
}
