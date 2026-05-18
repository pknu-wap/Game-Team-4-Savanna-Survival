using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingUiManager : MonoBehaviour
{
    public GameObject settingPanel;

    private bool isOpen = false;

    private void Start()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSetting();
        }
    }

    public void ToggleSetting()
    {
        isOpen = !isOpen;
        settingPanel.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;
    }

    public void OnStayButton()
    {
        isOpen = false;
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnExitToGameButton()
    {
        isOpen = false;
        settingPanel.SetActive(false);
        Time.timeScale = 0f;
        SceneManager.LoadScene("Start Scene");
    }
}