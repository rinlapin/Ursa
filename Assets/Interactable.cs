using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Prefab to spawn in place of the original object")]
    [SerializeField] private GameObject solvedObject;

    #region obsolete

    [Header("Constellation prefab to 'feed' into PuzzleGroup")]
    [SerializeField] private GameObject constellation;
    public GameObject Constellation => constellation;

    #endregion
    
    [SerializeField] private ConstellationData constellationData;
    
    public void TryInteract()
    {
        PuzzleManager.instance.ActiveData = constellationData;
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
