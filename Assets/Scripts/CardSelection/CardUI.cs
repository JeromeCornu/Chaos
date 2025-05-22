using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image borderImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = transform.localScale;
        Unhighlight(); // Setup initial
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
