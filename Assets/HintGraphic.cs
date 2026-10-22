using System;
using DG.Tweening;
using UnityEngine;

public class HintGraphic : MonoBehaviour
{
    public static HintGraphic instance;

    public CanvasGroup right, left;
    private CanvasGroup _activeArrow;
    
    public bool usedOnce = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public Transform LocateInteractable()
    {
        var interactable = FindAnyObjectByType<ForgottenInteractable>();
        return interactable?.transform;
    }

    public void Show(Action onComplete)
    {
        var found = LocateInteractable();
        var player = FindAnyObjectByType<PlayerMovement>();
        if (found != null)
        {
            usedOnce = true;
            
            if (found.transform.position.z > player.transform.position.z)
            {
                _activeArrow = right;
            }
            else
            {
                _activeArrow = left;
            }

            SoundManager.Instance.PlayEvent(SoundManager.Instance.playNavStar);
            _activeArrow.DOFade(1, 0.2f).OnComplete(() =>
            {
                _activeArrow.DOFade(0, 1.8f).OnComplete(() => onComplete?.Invoke());
            });
        }
        else
        {
            onComplete?.Invoke();
        }
    }
}
