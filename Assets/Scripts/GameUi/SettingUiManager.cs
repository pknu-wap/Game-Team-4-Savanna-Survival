using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingUiManager : MonoBehaviour
{
    public GameObject settingUI;
    public GameObject startUI;
    public GameObject gameUI;
    void Start()
    {
        settingUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void OnContinueButton()
    {
        settingUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnExitButton()
    {
        Time.timeScale = 0f;
        startUI.SetActive(true);
        gameUI.SetActive(false);
        settingUI.SetActive(false);
    }
}