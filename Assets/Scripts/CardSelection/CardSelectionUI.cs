using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CardSelectionUI : MonoBehaviour
{
    [SerializeField] private List<CardUI> cartes = new List<CardUI>();
    private int currentIndex = 0;

    void Start()
    {
        HighlightCard(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MoveSelection(1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MoveSelection(-1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            cartes[currentIndex].SelectCard();
    }

    void MoveSelection(int direction)
    {
        cartes[currentIndex].Unhighlight();
        currentIndex = (currentIndex + direction + cartes.Count) % cartes.Count;
        HighlightCard(currentIndex);
    }

    void HighlightCard(int index)
    {
        cartes[index].Highlight();
    }
}
