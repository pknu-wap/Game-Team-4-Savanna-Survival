using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUiManager : MonoBehaviour
{
    public void OnStartButton()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("Main Scene");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}