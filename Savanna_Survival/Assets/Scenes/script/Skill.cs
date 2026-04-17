using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Skill : MonoBehaviour
{
    public Image cooldownBar;
    public SkillData skillData;
    public TextMeshProUGUI skillText;

    private float lastUsedTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseSkill();
        }

        UpdateUI();
    }

    void UseSkill()
    {
        if (Time.time < lastUsedTime + skillData.cooldown)
        {
            Debug.Log("쿨타임 중!");
            return;
        }

        lastUsedTime = Time.time;

        Debug.Log("스킬 사용: " + skillData.skillName);
    }

  void UpdateUI()
{ 
    float remaining = (lastUsedTime + skillData.cooldown) - Time.time;
    if (remaining < 0) remaining = 0;

    float ratio = 1 - (remaining / skillData.cooldown);

    cooldownBar.fillAmount = ratio;

    skillText.text =
        skillData.skillName + "\n" +
        "데미지: " + skillData.damage + "\n" +
        "쿨타임: " + remaining.ToString("F1");

}

}