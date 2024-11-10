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
        // UpdateCountText(); 
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
                Debug.Log ("Invalid CardData Type: " + cardData.cardType);
                return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // cardList.AddRange(cardDB.cardList);
        // Shuffle();
        // UpdateCountText(); 
    }

    // Update is called once per frame
    void Update()
    {
        
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
        // UpdateCountText();

        return drawnCard;
    }

    public Card Peek()
    {
        if(cardList.Count < 1) { return null;}
        Card drawnCard = cardList[0];

        return drawnCard;
    }

    // void UpdateCountText()
    // {
    //     deckCountTMP.text = cardList.Count.ToString();
    //     discardCountTMP.text = discardPile.Count.ToString();
    // }

    public void Discard(Card card)
    {
        discardPile.Add(card);
        // UpdateCountText();
    }

    public void ShuffleHandBackIn(List<Card> hand)
    {
        cardList.AddRange(hand);
        Shuffle();
    }

}
