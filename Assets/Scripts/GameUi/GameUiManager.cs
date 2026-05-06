using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject startUI;
    public GameObject gameUI;
    public GameObject settingUI;

    void Start()
    {
        ShowStartUI();

        if (SceneManager.GetActiveScene().name == "map-time_system")
        {
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingUI();
        }
    }

    public void ShowStartUI()
    {
        startUI.SetActive(true);
        gameUI.SetActive(false);
        settingUI.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Start Scene")
        {
            Time.timeScale = 0f;
        }
    }

    public void OnStartButton()
    {
        startUI.SetActive(false);
        gameUI.SetActive(true);
        settingUI.SetActive(false);

        Time.timeScale = 1f;
    }

    public void ToggleSettingUI()
    {
        bool isSettingOpen = settingUI.activeSelf;

        if (isSettingOpen)
        {
            settingUI.SetActive(false);
            gameUI.SetActive(true);

            Time.timeScale = 1f;
        }
        else
        {
            settingUI.SetActive(true);
            gameUI.SetActive(false);

            Time.timeScale = 0f;
        }
    }

    public void CloseSettingUI()
    {
        settingUI.SetActive(false);
        gameUI.SetActive(true);

        Time.timeScale = 1f;
    }

    public void OnEndButton()
    {
        Application.Quit();
    }
}