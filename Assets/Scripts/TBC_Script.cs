using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TBC_Script : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    public void BacktoMM()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}