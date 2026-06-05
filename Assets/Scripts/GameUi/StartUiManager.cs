using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUiManager : MonoBehaviour
{
    public void OnStartButton()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("Main Temp Scene");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}