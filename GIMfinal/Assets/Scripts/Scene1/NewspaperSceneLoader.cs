using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class NewspaperSceneLoader : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private bool sceneLoaded = false;

    private void Update()
    {
        if (sceneLoaded) return;

        if (director.time >= director.duration - 0.1f)
        {
            sceneLoaded = true;

            StoryState.Chapter = 2;

            SceneManager.LoadScene("Office");
        }
    }
}