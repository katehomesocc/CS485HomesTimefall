using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Deck : MonoBehaviour
{
    public CardDatabase cardDB;
    public List<Card> cardList = new List<Card>();

    public List<Card> discardPile = new List<Card>();

    BattleManager battleManager;


    private void Awake() {

        foreach (var cardData in cardDB.cardList)
        {
            Card card = GetCardFromCardData(cardData);
            if(card == null)
            {
                Debug.Log("Card is null");
            }
            cardList.Add(card);
        }
        Shuffle();
    }

    private void Start()
    {
        battleManager = BattleManager.Instance;
    }

    private Card GetCardFromCardData(CardData cardData)
    {
        switch (cardData.cardType)
        {
           case CardType.AGENT:
                AgentCard agentCard = new AgentCard((AgentCardData) cardData);
                return agentCard;
            case CardType.ESSENCE:
                EssenceCard essenceCard = new EssenceCard((EssenceCardData)cardData);
                return essenceCard;
            case CardType.EVENT:
                EventCard eventCard = new EventCard((EventCardData)cardData);
                return eventCard;
            default:
            //Error handling
                Debug.LogError("Invalid CardData Type: " + cardData.cardType);
                return null;
        }
    }

    // Fisher-Yates shuffle: http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    public void Shuffle()
    {
        if(cardList.Count < 1) { return;}
        for (int i = cardList.Count - 1; i >= 0; --i)
        {
            int j = Random.Range(0, i + 1);

            Card tmp = cardList[i];
            cardList[i] = cardList[j];
            cardList[j] = tmp;
        }
    } 

    public Card Draw()
    {
        if(cardList.Count < 1) { return null;}
        Card drawnCard = cardList[0];
        cardList.RemoveAt(0);

        return drawnCard;
    }

    public Card Peek()
    {
        if(cardList.Count < 1) { return null;}
        Card drawnCard = cardList[0];

        return drawnCard;
    }

    public void Discard(Card card)
    {
        discardPile.Add(card);
    }

    public void ShuffleHandBackIn(List<Card> hand)
    {
        cardList.AddRange(hand);
        Shuffle();
    }

    public void ShowDiscardInventory()
    {
        battleManager.SetAndShowInventory(discardPile);
    }

}
