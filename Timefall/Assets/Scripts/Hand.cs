using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public static string LOCATION = "HAND";
    public Deck playerDeck;
    public Deck timelineDeck;
    public Button drawPlayerButton;
    public Button shufflePlayerButton;
    public Button drawTimelineButton;
    public Button shuffleTimelineButton;
    public RectTransform  handPanel;

    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;

    public Transform expandedViewTransform;

    public BoardManager boardManager;

    public TurnManager turnManager;

    // Start is called before the first frame update
    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();

		drawPlayerButton.onClick.AddListener(DrawFromPlayerDeck);

		shufflePlayerButton.onClick.AddListener(ShufflePlayerDeck);

		drawTimelineButton.onClick.AddListener(DrawFromTimelineDeck);

		shuffleTimelineButton.onClick.AddListener(ShuffleTimelineDeck);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawFromPlayerDeck()
    {
        DrawFromDeck(playerDeck);
    }



    void ShufflePlayerDeck()
    {
        ShuffleDeck(playerDeck);
    }

    void DrawFromDeck(Deck targetDeck){
        Card card = targetDeck.Draw();

        if(card == null){ return;}

        CardType cardType = card.cardType;

        // Debug.Log (card.ToString());

        GameObject obj = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                if(agentDC == null){return;} 
                agentDC.SetCard(card);
                agentDC.InstantiateInHand(transform);

                break;
            case CardType.ESSENCE:
                obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

                if(essenceDC == null){return;} 
                essenceDC.SetCard(card);
                essenceDC.InstantiateInHand(transform);
                break;
            case CardType.EVENT:
                obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return;} 
                eventDC.SetCard(card);
                eventDC.InstantiateInHand(transform);
                break;
            default:
            //Error handling
                Debug.Log ("Invalid Card Type: " + cardType);
                return;
        }

        // obj.transform.SetParent(transform);
        //obj.transform.localScale =  new Vector3(0.55f, 0.55f, 0.55f);
        
	}

    void ShuffleDeck(Deck targetDeck){
        targetDeck.Shuffle();
	}

    public CardDisplay ExpandCardView(Card card)
    {
        if(card == null){ return null;}

        CardType cardType = card.cardType;

        // Debug.Log (card.ToString());

        GameObject obj = null;

        CardDisplay displayToReturn = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                if(agentDC == null){return null;} 
                agentDC.SetCard(card);
                agentDC.Place(expandedViewTransform, "EXPAND");

                displayToReturn = agentDC;

                break;
            case CardType.ESSENCE:
                obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

                if(essenceDC == null){return null;} 
                essenceDC.SetCard(card);
                essenceDC.Place(expandedViewTransform, "EXPAND");

                displayToReturn = essenceDC;

                break;
            case CardType.EVENT:
                obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return null;} 
                eventDC.SetCard(card);
                eventDC.Place(expandedViewTransform, "EXPAND");

                displayToReturn = eventDC;

                break;
            default:
            //Error handling
                Debug.Log ("Invalid Card Type: " + cardType);
                return null;
        }

        obj.transform.SetParent(expandedViewTransform, false);

        return displayToReturn;
    }

    public void CloseExpandCardView()
    {
        while (expandedViewTransform.childCount > 0) {
            DestroyImmediate(expandedViewTransform.GetChild(0).gameObject);
        }
    }

        void ShuffleTimelineDeck()
    {
        ShuffleDeck(timelineDeck);
    }

    public void DrawFromTimelineDeck()
    {
        Debug.Log("Drawing from tiemline deck!");
        Card card = timelineDeck.Draw();

        if(card == null){ return;}
        Debug.Log("card exists!");

        CardDisplay display = ExpandCardView(card);

        display.playState = CardPlayState.IntialTimelineDraw;

        // display.OnPointerClick.AddListener(PlayTimelineCard(display));
    }

     public void PlayTimelineCard(CardDisplay display)
    {
        if(display == null){ return;}

        boardManager.PlaceTimelineEventForTurn(display);
    }

    public bool CanPlayCard(Card card)
    {
        return turnManager.CanPlayCard(card);
    }

}
