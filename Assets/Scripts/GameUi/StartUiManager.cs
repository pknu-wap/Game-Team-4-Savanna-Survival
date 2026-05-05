using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUiManager : MonoBehaviour
{
    public GameObject startUi;

    public GameObject timeUi;

    void Start()
    {
        Time.timeScale = 0f;
        startUi.SetActive(true);

        timeUi = GameObject.Find("TimeCanvas");

        if (timeUi != null)
        {
            timeUi.SetActive(false);
        }
    }

    public void OnStartButton()
    {
        if (timeUi == null)
        {
            timeUi = GameObject.Find("TimeCanvas");
        }

        if (timeUi != null)
        {
            timeUi.SetActive(true);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("map-time_system");
    }

    public void OnEndButton()
    {
        Application.Quit();
    }
}