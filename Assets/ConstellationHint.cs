using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstellationHint : MonoBehaviour
{
    private Tweener _flash;
    private List<GameObject> _clones;
    private CanvasGroup _canvasGroup;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _flash = _canvasGroup.
            DOFade(1, 0.5f).
            SetLoops(2, LoopType.Yoyo).
            SetAutoKill(false).
            Pause().
            OnComplete(() =>
            {
                PuzzleManager.state = PuzzleState.Idle;
            });
    }

    public void Setup(Transform target)
    {
        _clones = new List<GameObject>();
        foreach (var star in target.GetComponentsInChildren<Star>())
        {
            var clone = Instantiate(
                star.gameObject,
                star.transform.position,
                star.transform.rotation,
                transform);
            _clones.Add(clone);
        }
    }

    public void Activate()
    {
        _flash.Restart();
        _flash.Play();
    }

}
