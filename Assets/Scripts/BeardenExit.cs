using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger: MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SceneName";
    [SerializeField] private bool loadOnTriggerEnter = true;


    private void OnTriggerEnter(Collider other)
    {
        if (!loadOnTriggerEnter) return;
        if (other.CompareTag("Player"))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}