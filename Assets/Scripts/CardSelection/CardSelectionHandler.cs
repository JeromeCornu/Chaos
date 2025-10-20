using System;
using System.Collections.Generic;
using System.Linq;
using GameState;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CardSelectionHandler : NetworkBehaviour
{
    public static CardSelectionHandler Instance;
    [SerializeField] public CardNavigationUI cardNavigationUI;
    
    private Inputs inputs;
    
    private List<CardData> allCardOptions = new(); // list of all SO card 

    private List<CardData> currentCardSelection = new();
    
    [SyncVar] public uint cardChooserNetId;
    private int currentCardIndex = 0;
    private int cardCount = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        inputs = new Inputs();
    }

    private void Start()
    {
        NetworkServer.Spawn(gameObject);
        
        // Load card SOs
        if (allCardOptions.Count == 0)
        {
            allCardOptions = new List<CardData>(Resources.LoadAll<CardData>("SO_Card"));
            Debug.Log($"[GameManager] Loaded {allCardOptions.Count} cards from Resources/SO_Card/");
        }
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }
    
    [ClientRpc]
    public void RpcShowCardSelectionUI()
    {
        if (CardNavigationUI.Instance != null)
        {
            CardNavigationUI.Instance.Show(cardChooserNetId);
        }
    }
        
    public void HighlightCard_Server(int index)
    {
        RpcHighlightCard(index);
    }
        
    [ClientRpc]
    private void RpcHighlightCard(int index)
    {
        if (CardNavigationUI.Instance != null)
            CardNavigationUI.Instance.HighlightCard(index);
    }

    // public void GoToCardChoosePhase(uint loserNetId)
    // {
    //     GameManager.Instance.GoToNextState(); // update all clients
    //     RpcShowCardSelectionUI(loserNetId, 0); 
    // }

    public void GenerateCardChoices()
    {
        currentCardSelection.Clear();

        List<CardData> available = new List<CardData>(allCardOptions);

        for (int i = 0; i < 5; i++)
        {
            int index = Random.Range(0, available.Count);
            currentCardSelection.Add(available[index]);
            available.RemoveAt(index);
        }

        RpcDistributeCards(currentCardSelection.Select(c => allCardOptions.IndexOf(c)).ToArray());
    }

    [ClientRpc]
    private void RpcDistributeCards(int[] indexes)
    {
        List<CardData> selected = new();
        foreach (int i in indexes)
            selected.Add(allCardOptions[i]);

        Debug.Log("[GameManager] RpcDistributeCards selected: " + string.Join(", ", selected.Select(c => c.cardName)));

        CardNavigationUI.Instance.LoadCards(selected);
    }

    [Command(requiresAuthority = false)]
    public void HideCards()
    {
        HideCards_RPC();
    }

    [ClientRpc]
    private void HideCards_RPC()
    {
        CardNavigationUI.Instance.Hide();
    }
}
