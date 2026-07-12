using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstellationHint : MonoBehaviour
{
    private class StarClone
    {
        public GameObject GameObject;
        public bool IsMatched = false;
        public StarClone(GameObject gObject) => GameObject = gObject;
    }
    
    private Tweener _flash;
    private Dictionary<Star, StarClone> _clones;
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
        _clones = new Dictionary<Star, StarClone>();
        foreach (var star in target.GetComponentsInChildren<Star>())
        {
            var clone = Instantiate(
                star.gameObject,
                star.transform.position,
                star.transform.rotation,
                transform);
            _clones[star] = new StarClone(clone);
        }
    }

    public void Check()
    {
        _flash.Restart();
        _flash
            .Play()
            .OnComplete(() =>
            {
                if (AreStarsAligned())
                {
                    PuzzleManager.instance.Solve();
                }
            });
    }

    private bool AreStarsAligned()
    {
        foreach (var pair in _clones)
        {
            var star = pair.Key;
            var clone = pair.Value;
            
            if (Mathf.Abs(star.transform.position.x - clone.GameObject.transform.position.x) < ConstellationFramework.instance.graceWindow &&  
                Mathf.Abs(star.transform.position.y - clone.GameObject.transform.position.y) < ConstellationFramework.instance.graceWindow)
            {
                clone.IsMatched = true;
                Debug.Log($"Star {star.gameObject.name} matched");
            }
            else
            {
                clone.IsMatched = false;
            }
        }

        foreach (var clone in _clones.Values)
        {
            if (!clone.IsMatched)
            {
                return false;
            }
        }

        return true;
    }
}
