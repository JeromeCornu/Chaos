using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardNavigationUI : MonoBehaviour
{
    public static CardNavigationUI Instance;

    [Header("UI References")]
    [SerializeField] private GameObject contentRoot;         // "Content"
    [SerializeField] private Transform cardsListContainer;   // "CardsList"

    [Header("Input")]
    private Inputs inputActions;

    [Header("Navigation")]
    private List<CardUI> cards = new();
    private int currentIndex = 0;
    private float moveCooldown = 0.3f;
    private float lastMoveTime = 0f;

    private bool canNavigate = false;
    private uint localNetId = 0;
    private uint authorizedNetId = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inputActions = new Inputs();

        if (Mirror.NetworkClient.localPlayer != null)
            localNetId = Mirror.NetworkClient.localPlayer.netId;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        // Get all cards from the container
        cards.Clear();
        foreach (Transform child in cardsListContainer)
        {
            if (child.TryGetComponent(out CardUI card))
                cards.Add(card);
        }

        contentRoot.SetActive(false);
    }

    public void Show(uint netIdOfAuthorizedPlayer)
    {
        authorizedNetId = netIdOfAuthorizedPlayer;
        canNavigate = localNetId == authorizedNetId;

        currentIndex = 0;
        contentRoot.SetActive(true);
        HighlightCard(currentIndex);
    }

    public void Hide()
    {
        contentRoot.SetActive(false);
        canNavigate = false;
    }

    private void Update()
    {
        if (!canNavigate || !contentRoot.activeSelf)
            return;

        float axis = inputActions.PlayerInputs.LeftRight.ReadValue<float>();

        if (Time.time - lastMoveTime > moveCooldown)
        {
            if (axis > 0.5f)
            {
                MoveCursor(1);
                lastMoveTime = Time.time;
            }
            else if (axis < -0.5f)
            {
                MoveCursor(-1);
                lastMoveTime = Time.time;
            }
        }

        if (inputActions.PlayerInputs.Jump.triggered)
        {
            SelectCurrentCard();
        }

    }

    private void MoveCursor(int direction)
    {
        int newIndex = (currentIndex + direction + cards.Count) % cards.Count;
        HighlightCard(newIndex);
    }

    public void HighlightCard(int index)
    {
        if (index < 0 || index >= cards.Count) return;

        for (int i = 0; i < cards.Count; i++)
            cards[i].Unhighlight();

        cards[index].Highlight();

        currentIndex = index;
    }

    private void SelectCurrentCard()
    {
        if (cards.Count == 0 || currentIndex < 0 || currentIndex >= cards.Count)
            return;

        Debug.Log($"Card selected: {cards[currentIndex].gameObject.name}");

        // play selection animation then hide the UI
        cards[currentIndex].PlaySelectAnimation(() =>
        {
            Hide();
        });
    }



    public int TotalCards => cards.Count;

}
