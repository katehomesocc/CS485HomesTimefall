using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckDisplay : MonoBehaviour
{
    public TMP_Text deckCountTMP;
    public TMP_Text discardCountTMP;
    public Image border;

    public Deck deck;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateCountText()
    {
        deckCountTMP.text = deck.cardList.Count.ToString();
        discardCountTMP.text = deck.discardPile.Count.ToString();
    }

    public void SetPlayerDeck(Faction faction, Deck playerDeck)
    {
        this.deck = playerDeck;
        border.color = CardDisplay.GetFactionColor(faction);
        UpdateCountText();
    }
}
