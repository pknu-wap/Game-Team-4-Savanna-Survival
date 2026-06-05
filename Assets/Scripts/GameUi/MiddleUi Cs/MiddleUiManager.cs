using UnityEngine;
using UnityEngine.SceneManagement;

public class MiddleUiManager : MonoBehaviour
{
    public void OnBackButton()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("Start Scene");
    }

    public void OnStartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Temp Scene");
    }
}
