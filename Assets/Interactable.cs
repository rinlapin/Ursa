using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject solvedObject;
    
    public void TryInteract()
    {
        CanvasManager.instance.ActiveGroupType = CanvasGroupType.Puzzle;
        PuzzleManager.instance.activeInteractable = this;
    }

    public void Solve()
    {
        if (solvedObject)
        {
            var newSolvedObject = Instantiate(solvedObject);
            newSolvedObject.transform.position = gameObject.transform.position;
        }
        Destroy(gameObject);
    }
}
