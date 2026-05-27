using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SucceedUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject succeedPanel;
    [SerializeField] private SucceedUI[] succeedSlots;
    [SerializeField] private TMP_Text selectedCountText;
    [SerializeField] private Button succeedButton;

    [Header("Script")]
    [SerializeField] private EquipmentInventory equipmentInventory;
    [SerializeField] private SelectSucceedItem selectSucceedItem;
    [FormerlySerializedAs("itemInfoUI")]
    [SerializeField] private ItemInfoUI inventoryItemInfoUI;
    [SerializeField] private MaxSucceedSelectionNotice maxSelectionNotice;

    [Header("Option")]
    [SerializeField] private bool includeEquippedItems = true;

    private void Awake()
    {
        setAllSlotsActive(false);

        if (succeedPanel != null)
        {
            succeedPanel.SetActive(false);
        }
    }

    public void triggerSucceedUI()
    {
        initialize();

        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        openSucceedUI();
    }

    private void initialize()
    {
        if (selectSucceedItem != null)
        {
            selectSucceedItem.onSelectionChanged += refreshSelectedState;
            selectSucceedItem.onMaxSelectionBlocked += showMaxSelectionNotice;
        }

        if (succeedButton != null)
        {
            succeedButton.onClick.RemoveListener(closeSucceedUI);
            succeedButton.onClick.AddListener(closeSucceedUI);
        }
    }

    private void OnDestroy()
    {
        releaseRuntimeLinks();
    }

    private void openSucceedUI()
    {
        if (succeedPanel != null)
        {
            succeedPanel.SetActive(true);
        }

        if (selectSucceedItem != null)
        {
            selectSucceedItem.clearSelection();
        }

        refreshSucceedSlots();
    }

    public void closeSucceedUI()
    {
        if (inventoryItemInfoUI != null)
        {
            inventoryItemInfoUI.hideEquipmentInfo();
        }

        if (succeedPanel != null)
        {
            succeedPanel.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        releaseRuntimeLinks();
    }

    public void onSlotHover(SucceedUI slotUI)
    {
        if (slotUI == null || slotUI.Equipment == null || inventoryItemInfoUI == null)
        {
            return;
        }

        inventoryItemInfoUI.openEquipmentInfo(slotUI.Equipment);
    }

    public void onSlotExit(SucceedUI slotUI)
    {
        if (inventoryItemInfoUI != null)
        {
            inventoryItemInfoUI.hideEquipmentInfo();
        }
    }

    public void onSlotClick(SucceedUI slotUI)
    {
        if (slotUI == null || selectSucceedItem == null)
        {
            return;
        }

        selectSucceedItem.toggleSelect(slotUI.Equipment);
        refreshSelectedState();
    }

    private void refreshSucceedSlots()
    {
        if (equipmentInventory == null || succeedSlots == null || succeedSlots.Length <= 0)
        {
            refreshSelectedState();
            return;
        }

        List<EquipmentData> equipments = equipmentInventory.getCurrentEquipments(includeEquippedItems);
        int activeSlotCount = Mathf.Min(equipments.Count, succeedSlots.Length);

        for (int i = 0; i < succeedSlots.Length; ++i)
        {
            if (succeedSlots[i] == null)
            {
                continue;
            }

            bool hasEquipment = i < activeSlotCount;
            succeedSlots[i].gameObject.SetActive(hasEquipment);
            succeedSlots[i].initialize(this, hasEquipment ? equipments[i] : null);
        }

        if (equipments.Count > succeedSlots.Length)
        {
            Debug.LogWarning("계승 UI 슬롯 개수가 현재 장비 개수보다 적습니다.");
        }

        refreshSelectedState();
    }

    private void refreshSelectedState()
    {
        if (selectSucceedItem != null && selectedCountText != null)
        {
            selectedCountText.text = "선택 가능: " + selectSucceedItem.SelectedCount + " / " + selectSucceedItem.MaxSelectCount;
        }

        if (succeedSlots == null)
        {
            return;
        }

        for (int i = 0; i < succeedSlots.Length; ++i)
        {
            if (succeedSlots[i] == null)
            {
                continue;
            }

            bool isSelected = succeedSlots[i].gameObject.activeSelf
                && selectSucceedItem != null
                && selectSucceedItem.isSelected(succeedSlots[i].Equipment);

            succeedSlots[i].setSelected(isSelected);
        }
    }

    private void showMaxSelectionNotice()
    {
        if (maxSelectionNotice != null)
        {
            maxSelectionNotice.play();
        }
    }

    private void setAllSlotsActive(bool isActive)
    {
        if (succeedSlots == null)
        {
            return;
        }

        for (int i = 0; i < succeedSlots.Length; ++i)
        {
            if (succeedSlots[i] != null)
            {
                succeedSlots[i].initialize(this, null);
                succeedSlots[i].gameObject.SetActive(isActive);
            }
        }
    }

    private void releaseRuntimeLinks()
    {
        if (selectSucceedItem != null)
        {
            selectSucceedItem.onSelectionChanged -= refreshSelectedState;
            selectSucceedItem.onMaxSelectionBlocked -= showMaxSelectionNotice;
        }

        if (succeedButton != null)
        {
            succeedButton.onClick.RemoveListener(closeSucceedUI);
        }
    }
}
