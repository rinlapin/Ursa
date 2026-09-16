using System;
using UnityEngine;

public class ForgottenInteractable : Interactable
{
        [SerializeField] private SolvedInteractable solvedInteractable;
        [field: SerializeField] public ConstellationData Data { get; private set; }
        
        public Action<ForgottenInteractable> OnSolve;
        public Action<SolvedInteractable> OnSpawnSolvedInteractable;

        public void Solve()
        {
                print("solved!");
                OnSolve?.Invoke(this);
                var newSolvedInteractable = Instantiate(solvedInteractable, transform.position, transform.rotation);
                GameManager.Instance.ClosestInteractable = newSolvedInteractable;
                Destroy(gameObject);
        }
}
