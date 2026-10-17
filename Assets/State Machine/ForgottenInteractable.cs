using System;
using UnityEngine;
using UnityEngine.Events;

public class ForgottenInteractable : Interactable
{
        [SerializeField] private SolvedInteractable solvedInteractable;
        [field: SerializeField] public ConstellationData Data { get; private set; }
        
        public UnityEvent<ForgottenInteractable> OnSolve;
        public Action<SolvedInteractable> OnSpawnSolvedInteractable;

        public void Solve()
        {
                SoundManager.Instance.PlayEvent(SoundManager.Instance.playRebirth);
                print("solved!");
                OnSolve?.Invoke(this);
                var newSolvedInteractable = Instantiate(solvedInteractable, transform.position, transform.rotation);
                GameManager.Instance.ClosestInteractable = newSolvedInteractable;
                Destroy(gameObject);
        }
}
