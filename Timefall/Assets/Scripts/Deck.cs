using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Deck : MonoBehaviour
{
    public CardDatabase cardDB;
    public List<Card> cardList = new List<Card>();

    public List<Card> discardPile = new List<Card>();

    public TMP_Text deckCountTMP;

    // Start is called before the first frame update
    void Start()
    {
        cardList.AddRange(cardDB.cardList);
        Shuffle();
        UpdateCountText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Fisher-Yates shuffle: http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    void Shuffle()
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
        Debug.Log ("Drawing from deck!");
        Card drawnCard = cardList[0];
        cardList.RemoveAt(0);
        UpdateCountText();

        return drawnCard;
    }

    public Card Peek()
    {
        if(cardList.Count < 1) { return null;}
        Card drawnCard = cardList[0];

        return drawnCard;
    }

    void UpdateCountText()
    {
        deckCountTMP.text = cardList.Count.ToString();
    }

}
