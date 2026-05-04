using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject augmentPanel;
    [SerializeField] private AugmentChoiceButton[] choiceButtons;

    private int currentLevel;
    private bool isOpen;

    private void Awake()
    {
        Instance = this;
        if (augmentPanel != null)
            augmentPanel.SetActive(false);
    }

    private void Start()
    {
        OpenAugment(1);
    }

    /// 게임을 중단하고 증강 3개를 보여준다.
    public void OpenAugment(int level)
    {
        if (isOpen) return;
        isOpen = true;
        currentLevel = level;

        Time.timeScale = 0f;

        List<AugmentDTO> choices = PickRandom(3);

        augmentPanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].Setup(choices[i], currentLevel, OnAugmentSelected);
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// 플레이어가 증강을 선택했을 때 호출된다.
    private void OnAugmentSelected(AugmentDTO selected)
    {
        AugmentPowerDTO power = selected.Power.Find(p => p.Level == currentLevel);
        if (power != null)
        {
            Debug.Log($"증강 선택: {selected.Name} (Lv{currentLevel}, +{power.Amount})");
        }

        augmentPanel.SetActive(false);
        isOpen = false;
        Time.timeScale = 1f;
    }

    /// 전체 증강 목록에서 count개를 중복 없이 랜덤 선택한다.
    private List<AugmentDTO> PickRandom(int count)
    {
        List<AugmentDTO> all = new List<AugmentDTO>(AugmentRepository.GetAll().Values);

        // AI 도움 받음: 피셔-에이츠 셔플 알고리즘. 리스트/배열 무작위로 섞기.
        for (int i = all.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (all[i], all[j]) = (all[j], all[i]);
        }
        
        
        return all.Take(Mathf.Min(count, all.Count)).ToList();
    }
}