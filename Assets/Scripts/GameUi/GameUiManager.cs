using UnityEngine;

public class GameUiManager : MonoBehaviour
{
    public GameObject gameUI;
    public GameObject settingUI;

    void Start()
    {
        gameUI.SetActive(true);
        settingUI.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingUI();
        }
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
}