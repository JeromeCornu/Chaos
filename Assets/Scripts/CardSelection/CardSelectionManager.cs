using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Utils;

namespace DefaultNamespace.CardSelection
{
    public class CardSelectionManager : NetworkBehaviour
    {
        public static CardSelectionManager Instance;
        
        [SyncVar] private uint cardChooserNetId;
        private int currentCardIndex = 0;
        private int cardCount = 0;

        public uint CardChooserNetId => cardChooserNetId;

        private List<CardData> allCardOptions = new(); // list of all SO card 

        private List<CardData> currentCardSelection = new();
        
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
        }
        
        public void Init()
        {
            // Load card SOs
            if (allCardOptions.Count == 0)
            {
                allCardOptions = new List<CardData>(Resources.LoadAll<CardData>("SO_Card"));
                Debug.Log($"[GameManager] Loaded {allCardOptions.Count} cards from Resources/SO_Card/");
            }
        }
        
        // card selection UI

        public void TriggerCardSelectionForPlayer(uint loserNetId)
        {
            cardChooserNetId = loserNetId;
            currentCardIndex = 0;

            if (CardNavigationUI.Instance != null)
                cardCount = CardNavigationUI.Instance.TotalCards;

            RpcShowCardSelectionUI(loserNetId, currentCardIndex);
        }

        [ClientRpc]
        public void RpcShowCardSelectionUI(uint interactorNetId, int highlightIndex)
        {
            if (CardNavigationUI.Instance != null)
            {
                CardNavigationUI.Instance.Show(interactorNetId);
                CardNavigationUI.Instance.HighlightCard(highlightIndex);
            }
        }

        public void MoveCardCursor(int direction)
        {
            if (cardCount == 0 && CardNavigationUI.Instance != null)
                cardCount = CardNavigationUI.Instance.TotalCards;

            currentCardIndex = (currentCardIndex + direction + cardCount) % cardCount;
            RpcHighlightCard(currentCardIndex);
        }

        [ClientRpc]
        private void RpcHighlightCard(int index)
        {
            if (CardNavigationUI.Instance != null)
                CardNavigationUI.Instance.HighlightCard(index);
        }

        public void GenerateCardChoices()
        {
            currentCardSelection = allCardOptions.GetRandomDistinctCount(5);

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

    }
}