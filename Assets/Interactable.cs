using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private GameObject solvedObject;
    
    public void TryInteract()
    {
        Debug.Log("Interacting with " + gameObject.name);
        CanvasManager.instance.ActiveGroup = CanvasGroupType.Puzzle;
        PuzzleManager.instance.activeInteractable = this;
    }

    public void Solve()
    {
        Debug.Log("Solving " + gameObject.name);
        
        if (solvedObject)
        {
            var newSolvedObject = Instantiate(solvedObject);
            newSolvedObject.transform.position = gameObject.transform.position;
        }
        Destroy(gameObject);
    }
}
