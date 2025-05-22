using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image border;
    [SerializeField] private Transform cardTransform;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color normalColor;

    private Vector3 originalScale;

    void Awake()
    {
        originalScale = cardTransform.localScale;
        border.color = normalColor;
    }

    public void Highlight()
    {
        border.DOColor(highlightColor, 0.2f);
        cardTransform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void Unhighlight()
    {
        border.DOColor(normalColor, 0.2f);
        cardTransform.DOScale(originalScale, 0.2f).SetEase(Ease.InBack);
    }

    public void SelectCard()
    {
        // Jouer une animation cool
        cardTransform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1);

        // TODO : envoyer la sélection au serveur si en multijoueur
        Debug.Log("Carte sélectionnée: " + gameObject.name);
    }
}
