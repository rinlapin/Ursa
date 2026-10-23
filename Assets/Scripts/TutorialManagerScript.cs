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

        public GameObject firstCollider;
        public GameObject secondCollider;


    [Serializable]
    public class DialogueLine
    {
        public string Text;
        public UnityEvent OnLineComplete;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == firstCollider ||
            collision.gameObject == secondCollider)
        {
            StartDialogue();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == firstCollider ||
            other.gameObject == secondCollider)
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (!hasTalked)
        {
            Debug.Log("Starting initial dialogue!");

            hasTalked = true;
        }
        else
        {
            Debug.Log("Starting repeated interaction dialogue!");

        }
    }
}