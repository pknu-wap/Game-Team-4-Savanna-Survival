using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadUiManager : MonoBehaviour
{
    public GameObject deadPanel;
    public GameObject settingPanel;
    public PlayerHp playerHp;

    private bool isDead = false;
    private bool hpCheckedOnce = false;
    private FieldInfo currentHpField;

    private void Start()
    {
        deadPanel.SetActive(false);
        Time.timeScale = 1f;

        if (playerHp == null)
        {
            playerHp = FindObjectOfType<PlayerHp>();
        }

        currentHpField = typeof(PlayerHp).GetField(
            "currentHp",
            BindingFlags.NonPublic | BindingFlags.Instance
        );
    }

     private void Update()
    {
        if (isDead) return;
        if (playerHp == null) return;
        if (currentHpField == null) return;

        float currentHp = (float)currentHpField.GetValue(playerHp);
        // currentfield의 저장된 값을 실수 형태로 저장

         if (!hpCheckedOnce)
        {
            hpCheckedOnce = true;
            return;
        }// 처음 Hp 검사는 무시 / 처음 currentHp 값이 0이 나와서 처음은 무시해야함
        Debug.Log("읽은 체력: " + currentHp);

        if (currentHp <= 0f)
        {
            OpenDeadUI();
        }
    }//currentHp의 값이 0보다 작을 떄, 즉 플레이어가 죽을 경우 사망 화면이 띄어짐

    private void OpenDeadUI()
    {
        isDead = true;
        deadPanel.SetActive(true);
        settingPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void OnGoEndSceneButton()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("End Scene");
    }
}