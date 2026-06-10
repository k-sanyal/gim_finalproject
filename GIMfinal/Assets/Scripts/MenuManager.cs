using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Office");
    }

    public void OpenCredits()
    {
        // show credits panel
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}