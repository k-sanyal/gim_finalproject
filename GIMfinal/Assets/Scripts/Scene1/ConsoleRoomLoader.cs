using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ConsoleRoomLoader : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private bool sceneLoaded = false;

    private void Update()
    {
        if (sceneLoaded) return;

        if (director.time >= director.duration - 0.1f)
        {
            sceneLoaded = true;

            Debug.Log("ConsoleRoom으로 이동!");

            SceneManager.LoadScene("ConsoleRoom");
        }
    }
}