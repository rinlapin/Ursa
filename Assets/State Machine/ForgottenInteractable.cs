using System;
using UnityEngine;
using UnityEngine.Events;

public class ForgottenInteractable : Interactable
{
        [field: SerializeField] public ConstellationData Data { get; private set; }
        
        public UnityEvent<ForgottenInteractable> OnSolve;
        public Action<SolvedInteractable> OnSpawnSolvedInteractable;

        public void Solve()
        {
                print("solved!");
                OnSolve?.Invoke(this);
                Destroy(gameObject);
        }
}
