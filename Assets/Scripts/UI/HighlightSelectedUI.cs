using UnityEngine;

public class HighlightSelectedUI : MonoBehaviour
{
    [SerializeField] private GameObject highlightObject;

    private void Awake()
    {
        setHighlighted(false);
    }

    public void setHighlighted(bool isHighlighted)
    {
        if (highlightObject != null)
        {
            highlightObject.SetActive(isHighlighted);
            return;
        }

        gameObject.SetActive(isHighlighted);
    }
}
