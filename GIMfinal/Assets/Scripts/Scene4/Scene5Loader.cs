using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene5Loader : MonoBehaviour
{
    public string nextSceneName = "GolfCourse";

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}