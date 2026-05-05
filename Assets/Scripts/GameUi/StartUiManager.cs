using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUiManager : MonoBehaviour
{
    public GameObject startUi;

    void Start()
    {
        Time.timeScale = 1f;
        startUi.SetActive(true);
    }
    public void OnStartButton()
    {
        SceneManager.LoadScene("map-time_system");

    }
    public void OnEndButton()
    {
        startUi.SetActive(false);
    }
}