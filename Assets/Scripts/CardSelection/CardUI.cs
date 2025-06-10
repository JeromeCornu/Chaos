using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image borderImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    private Vector3 originalScale;
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Transform statContainer;
    [SerializeField] private GameObject statLinePrefab;


    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Setup(CardData data)
    {
        Debug.Log($"[CardUI] Setting up card: {data.cardName}");

        titleText.text = data.cardName;

        if (data.cardImage != null)
            cardImage.sprite = data.cardImage;

        descriptionText.text = data.description;

        // Clear previous stat lines
        foreach (Transform child in statContainer)
            Destroy(child.gameObject);

        foreach (var modifier in data.statModifiers)
        {
            var statLine = Instantiate(statLinePrefab, statContainer);

            // Get the 2 direct children: name and value
            var nameText = statLine.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var valueText = statLine.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            if (nameText == null || valueText == null)
            {
                Debug.LogError("[CardUI] StatLine prefab is missing child TextMeshProUGUI elements.");
                continue;
            }

            nameText.text = modifier.stat.ToString();

            string valueStr = modifier.value.ToString();
            if (!valueStr.StartsWith("-"))
                valueStr = "+" + valueStr;

            valueText.text = valueStr;
        }
    }


    public void Highlight()
    {
        borderImage.color = highlightColor;
        transform.DOScale(originalScale * 1.1f, 0.25f).SetEase(Ease.OutBack);
    }

    public void Unhighlight()
    {
        borderImage.color = normalColor;
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.InBack);
    }

    public void PlaySelectAnimation(System.Action onComplete = null)
    {
        transform
            .DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1)
            .OnComplete(() => onComplete?.Invoke());
    }

}
