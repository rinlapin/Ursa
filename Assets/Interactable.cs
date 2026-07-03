using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Prefab to spawn in place of the original object")]
    [SerializeField] private GameObject solvedObject;
    
    [Header("Constellation prefab to 'feed' into PuzzleGroup")]
    [SerializeField] private GameObject constellation;
    public GameObject Constellation => constellation;
    
    public void TryInteract()
    {
        PuzzleManager.instance.Setup(this);
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
