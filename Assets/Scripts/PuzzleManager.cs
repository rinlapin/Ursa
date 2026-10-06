using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject puzzleVisuals;
    [field: SerializeField] public Transform Pivot { get; protected set; }
    [field: SerializeField] public CanvasGroup Hint { get; protected set; }
    [field: SerializeField] public CanvasGroup WinBackground { get; protected set; }
    [SerializeField] private GameObject starPrefab, hintStarPrefab;
    [SerializeField] private Camera puzzleCamera;
    
    private List<ForgottenInteractable> _forgottenInteractables = new List<ForgottenInteractable>();

    private void Awake()
    {
        _forgottenInteractables = FindObjectsByType<ForgottenInteractable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        foreach (var interactable in _forgottenInteractables)
        {
            interactable.OnSolve += Remove;
        }
        
        puzzleVisuals.SetActive(false);
        Hint.alpha = 0;
        
    }

    private void Remove(ForgottenInteractable interactable)
    {
        _forgottenInteractables.Remove(interactable);
    }

    public void SetupPuzzle(ForgottenInteractable newInteractable)
    {
        WinBackground.alpha = 0;

        foreach (Star star in Pivot.GetComponentsInChildren<Star>())
        {
            Destroy(star.gameObject);
        }

        foreach (Star star in Hint.GetComponentsInChildren<Star>())
        {
            Destroy(star.gameObject);
        }

        foreach (Vector3 position in newInteractable.Data.positions)
        {
            var newStar = Instantiate(starPrefab, Pivot);
            newStar.GetComponent<Star>().targetCamera = puzzleCamera;
            newStar.GetComponent<RectTransform>().anchoredPosition3D = position;
            
            var hintStar = Instantiate(hintStarPrefab, Hint.transform);
            hintStar.GetComponent<RectTransform>().anchoredPosition3D = position;
        }
        
        Pivot.rotation = Quaternion.Euler(Random.Range(-24, 24) * 15, Random.Range(-24, 24) * 15, Random.Range(-24, 24) * 15);
        
        puzzleVisuals.SetActive(true);
    }

    public void Close()
    {
        puzzleVisuals.SetActive(false);
    }
}