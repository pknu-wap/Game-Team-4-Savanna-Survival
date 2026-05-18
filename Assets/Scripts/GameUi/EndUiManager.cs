using UnityEngine;
using UnityEngine.SceneManagement;

public class EndUiManager : MonoBehaviour
{
    public GameObject playerReset;

    public void OnExitButton()
    {
        Time.timeScale = 0f;

        if (playerReset != null)
        {
            Destroy(playerReset);
        }

        SceneManager.LoadScene("Start Scene");
    }
}