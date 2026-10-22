using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueGroup;
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private float timeBetweenLetters = 0.1f;
    private List<DialogueLine> _currentDialogueLines = new();
    private SolvedInteractable _currentInteractable;

    public bool reachedEnd;
    public Action OnLineComplete;

    private Sequence _typeWord;

    private int _lineIndex;

    private void Awake()
    {
        dialogueGroup.SetActive(false);
        reachedEnd = false;
    }
    
    public void Close()
    {
        ProcessOnDialogueComplete();
        SetDialogue(null);
        dialogueGroup.SetActive(false);
    }

    public void SetDialogue(SolvedInteractable interactable)
    {
        if (interactable == null)
        {
            _currentInteractable = null;
            dialogue.text = "";
            _lineIndex = 0;
            _currentDialogueLines.Clear();
            dialogueGroup.SetActive(false);
            return;
        }

        _currentInteractable = interactable;
        dialogue.text = "";
        _lineIndex = 0;
        _currentDialogueLines = !_currentInteractable.hasTalked ? _currentInteractable.dialogueLines : new List<DialogueLine> { _currentInteractable.repeatedInteractionLine };
        dialogueGroup.SetActive(_currentDialogueLines != null);
    }
    
    public void NextLine()
    {
        WriteLine(_currentDialogueLines[_lineIndex]);
    }

    private void WriteLine(DialogueLine line)
    {
        dialogue.text = "";
        
        _typeWord = DOTween.Sequence();
        for (int i = 0; i < line.Text.Length; i++)
        {
            var letter = line.Text[i];
            _typeWord
                .AppendCallback(() => dialogue.text += letter)
                .AppendInterval(timeBetweenLetters);
        }

        _typeWord.OnComplete(() =>
        {
            OnLineComplete?.Invoke();
            _lineIndex++;
            reachedEnd = _lineIndex >= _currentDialogueLines.Count;
        });
        _typeWord.Play();
    }

    public void SkipAnimation()
    {
        _typeWord.Kill();
        dialogue.text = _currentDialogueLines[_lineIndex].Text;
        OnLineComplete?.Invoke();
        _lineIndex++;
        reachedEnd = _lineIndex >= _currentDialogueLines.Count;
    }

    public void ProcessAfterLineEvent()
    {
        
    }

    public void ProcessOnDialogueComplete()
    {
        _currentInteractable?.OnInitialDialogueComplete?.Invoke();
        _currentInteractable.hasTalked = true;
    }
}
