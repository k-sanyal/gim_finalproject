using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private bool sceneLoaded = false;

    private void Update()
    {
        if (sceneLoaded) return;

        if (director.time >= director.duration - 0.1f)
        {
            sceneLoaded = true;
            Debug.Log("씬 전환!");
            SceneManager.LoadScene("Newspaper");
        }
    }
}