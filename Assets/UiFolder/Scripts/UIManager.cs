using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject startUI;
    public GameObject gameUI;

    public void OnStartButton()
    {
        startUI.SetActive(false); 
        gameUI.SetActive(true);   

        Time.timeScale = 1f;
    }

    public void OnEndButton()
    {
        startUI.SetActive(false);
        gameUI.SetActive(false);

        Time.timeScale = 0f;
    }
}