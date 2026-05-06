using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUIManager : MonoBehaviour
{
    public GameObject startUI;

    void Start()
    {
        Time.timeScale = 0f;
        startUI.SetActive(true);
    }

    public void OnStartButton()
    {
        startUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnEndButton()
    {
        Application.Quit();
    }
}