using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUIManager : MonoBehaviour
{
    public GameObject startUI;

    void Start()
    {
        Time.timeScale = 1f;
        startUI.SetActive(true);
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene("map-time_system");
    }

    public void OnEndButton()
    {
        Application.Quit();
    }
}