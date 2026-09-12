using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Prefab to spawn in place of the original object")]
    [SerializeField] private GameObject solvedObject;

    #region obsolete

    [Header("Constellation prefab to 'feed' into PuzzleGroup")]
    [HideInInspector] [SerializeField] private GameObject constellation;
    public GameObject Constellation => constellation;

    #endregion
    
    [SerializeField] private ConstellationData data;
    public ConstellationData Data => data;
    
    public void TryInteract()
    {
        PuzzleManager.instance.ActiveInteractable = this;
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
