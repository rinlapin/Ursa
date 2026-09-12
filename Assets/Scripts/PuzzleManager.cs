using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleState state = PuzzleState.Idle;
    private Camera _mainCamera;

    private ConstellationData _activeData;

    public ConstellationData ActiveData
    {
        get => _activeData;
        set
        {
            _activeData = value;
            Player.state = _activeData ? PlayerState.Interacting : PlayerState.Idle;
            if (!value) return;
            ConstellationFramework.instance.Setup(_activeData);
        }
    }

    #region obsolete
    private GameObject _activeConstellation;
    public GameObject ActiveConstellation
    {
        get => _activeConstellation;
        set
        {
            if (_activeConstellation)
            {
                Destroy(_activeConstellation);
                _activeConstellation = null;
            }

            if (!value)
            {
                //ConstellationFramework.instance.Constellation = null;
                return;
            }
            
            _activeConstellation = Instantiate(value, ConstellationFramework.instance.canvas.transform);
            //ConstellationFramework.instance.Constellation = _activeConstellation.transform;
        }
    }
    #endregion

    public static PuzzleManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        _mainCamera = Camera.main;
    }

    private Interactable _activeInteractable;

    public Interactable ActiveInteractable
    {
        get =>  _activeInteractable;
        set
        {
            _activeInteractable = value;
            ActiveData = value ? value.Data : null;
        }
    }

    public void Exit()
    {
        ActiveInteractable = null;
        
        _mainCamera.gameObject.SetActive(true);
        ConstellationFramework.instance.gameObject.SetActive(false);

        CanvasManager.instance.ActiveGroupType = CanvasGroupType.None;
        Player.state = PlayerState.Idle;
    }

    public void Solve()
    {
        ActiveInteractable.Solve();
        PlayerInteraction.instance.closestInteractable = null;
        Exit();
    }
}

public enum PuzzleState
{
    Idle,
    Rotating,
    Hinting
}
