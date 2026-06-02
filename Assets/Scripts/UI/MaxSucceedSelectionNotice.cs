using System.Collections;
using TMPro;
using UnityEngine;

public class MaxSucceedSelectionNotice : MonoBehaviour
{
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private RectTransform countTextTransform;
    [SerializeField] private string maxMessage = "최대 선택 개수입니다.";
    [SerializeField] private float showSeconds = 1.2f;
    [SerializeField] private float shakeSeconds = 0.25f;
    [SerializeField] private float shakePower = 8f;

    private Coroutine warningCoroutine;
    private Coroutine shakeCoroutine;
    private Vector2 countTextOriginPosition;

    private void Awake()
    {
        if (countTextTransform != null)
        {
            countTextOriginPosition = countTextTransform.anchoredPosition;
        }

        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    public void play()
    {
        if (warningText != null)
        {
            warningText.text = maxMessage;
            warningText.gameObject.SetActive(true);

            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }
            warningCoroutine = StartCoroutine(hideWarningAfterDelay());
        }

        if (countTextTransform != null)
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
                countTextTransform.anchoredPosition = countTextOriginPosition;
            }
            shakeCoroutine = StartCoroutine(shakeCountText());
        }
    }

    private IEnumerator hideWarningAfterDelay()
    {
        yield return new WaitForSeconds(showSeconds);

        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }

        warningCoroutine = null;
    }

    private IEnumerator shakeCountText()
    {
        float elapsedSeconds = 0f;

        while (elapsedSeconds < shakeSeconds)
        {
            elapsedSeconds += Time.unscaledDeltaTime;
            float offsetX = Random.Range(-shakePower, shakePower);
            countTextTransform.anchoredPosition = countTextOriginPosition + new Vector2(offsetX, 0f);
            yield return null;
        }

        countTextTransform.anchoredPosition = countTextOriginPosition;
        shakeCoroutine = null;
    }
}
