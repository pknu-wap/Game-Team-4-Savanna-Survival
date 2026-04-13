using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class OptionUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject optionPanel;

    private bool isOpen = false;

    void Start()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleOption();
        }
    }

    public void ToggleOption()
    {
        isOpen = !isOpen;
        optionPanel.SetActive(isOpen);

        Time.timeScale = isOpen ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isOpen = false;
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitToStart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }
}