using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public static Hand Instance;
    public static string LOCATION = "HAND";

    public RectTransform  handPanel;

    public List<Card> cardsInHand;

    [Header("Managers")]
    public BattleManager battleManager;
    public BoardManager boardManager;

    public TurnManager turnManager;
    public Deck timelineDeck;
    public DeckDisplay playerDeckDisplay;

    [Header("Buttons")]
    public Button drawPlayerButton;
    public Button shufflePlayerButton;
    public Button drawTimelineButton;
    public Button shuffleTimelineButton;

    [Header("Card Display Prefabs")]
    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;

    [Header("Expanded Card View")]
    public Transform expandedHoverTransform;

    public Transform expandedStaticTransform;

    public List<CardDisplay> staticCards = new List<CardDisplay>();

    [Header("Hand State")]
    public HandState handState = HandState.NONE;
    public Card cardPlaying;
    public int cardPlayingIndex;

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        battleManager = FindObjectOfType<BattleManager>();

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
        DrawFromDeck(playerDeckDisplay.deck);
    }



    void ShufflePlayerDeck()
    {
        ShuffleDeck(playerDeckDisplay.deck);
    }

    void DrawFromDeck(Deck targetDeck){
        Card card = targetDeck.Draw();

        if(card == null){ return;}

        CardType cardType = card.data.cardType;

        GameObject obj = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                if(agentDC == null){return;} 
                AgentCard agentCard = (AgentCard) card;

                if(agentCard == null){return;} 
                agentDC.SetCard(agentCard);
                agentDC.InstantiateInHand(transform);
                agentDC.positionInHand = cardsInHand.Count;

                break;
            case CardType.ESSENCE:
                obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

                if(essenceDC == null){return;} 
                EssenceCard essenceCard = (EssenceCard) card;

                if(essenceCard == null){return;} 
                essenceDC.SetCard(essenceCard);
                essenceDC.InstantiateInHand(transform);
                essenceDC.positionInHand = cardsInHand.Count;

                break;
            case CardType.EVENT:
                obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return;} 
                EventCard eventCard = (EventCard) card;

                if(eventCard == null){return;} 
                eventDC.SetCard(eventCard);
                eventDC.InstantiateInHand(transform);
                eventDC.positionInHand = cardsInHand.Count;

                break;
            default:
            //Error handling
                Debug.LogError("Invalid CardData Type: " + cardType);
                return;
        }

        cardsInHand.Add(card);

        // obj.transform.SetParent(transform);
        //obj.transform.localScale =  new Vector3(0.55f, 0.55f, 0.55f);
        
	}

    void ShuffleDeck(Deck targetDeck){
        targetDeck.Shuffle();
	}

    public CardDisplay ExpandCardView(Card card, bool hoverClear)
    {
        if(card == null){ return null;}

        CardType cardType = card.data.cardType;

        GameObject obj = null;

        CardDisplay displayToReturn = null;

        Transform expandedViewTransform = hoverClear ? expandedHoverTransform :  expandedStaticTransform;

        switch(cardType) 
        {
            case CardType.AGENT:
                obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                if(agentDC == null){return null;} 
                AgentCard agentCard = (AgentCard) card;
                agentDC.SetCard(agentCard);
                agentDC.Place(expandedViewTransform, "EXPAND");

                displayToReturn = agentDC;

                break;
            case CardType.ESSENCE:
                obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

                if(essenceDC == null){return null;}
                EssenceCard essenceCard = (EssenceCard) card ;
                essenceDC.SetCard(essenceCard);
                essenceDC.Place(expandedViewTransform, "EXPAND");

                displayToReturn = essenceDC;

                break;
            case CardType.EVENT:
                obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return null;} 
                EventCard eventCard = (EventCard) card;
                eventDC.SetCard(eventCard);
                eventDC.Place(expandedViewTransform, "EXPAND");

                displayToReturn = eventDC;

                break;
            default:
            //Error handling
                Debug.LogError("Invalid CardData Type: " + cardType);
                return null;
        }

        obj.transform.SetParent(expandedViewTransform, false);

        if(!hoverClear)
        {
            staticCards.Add(displayToReturn);
        }

        return displayToReturn;
    }

    public void CloseExpandCardView()
    {
        while (expandedHoverTransform.childCount > 0) {
            
            DestroyImmediate(expandedHoverTransform.GetChild(0).gameObject);
        }
    }

        void ShuffleTimelineDeck()
    {
        ShuffleDeck(timelineDeck);
    }

    public void DrawFromTimelineDeck()
    {
        Card card = timelineDeck.Draw();

        if(card == null){ return;}

        CardDisplay display = ExpandCardView(card, false);

        display.playState = CardPlayState.START_TURN_DRAW_TIMELINE;
        SetHandState(HandState.START_TURN_DRAW_TIMELINE);
    }

    public void PlayInitialTimelineCard(CardDisplay display)
    {
        if(display == null){ return;}

        boardManager.PlaceTimelineEventForTurn(display);
        turnManager.SetVictoryPointUI();
        turnManager.EnableEndTurnButton();

        SetHandState(HandState.CHOOSING);
    }

    public void AutoPlayTimelineCard()
    {
        if(staticCards.Count == 1)
        {
            PlayInitialTimelineCard(staticCards[0]);
            staticCards.RemoveAt(0);
        }
    }

    public bool CanPlayCard(Card card)
    {
        return turnManager.CanPlayCard(card) && card.CanBePlayed();
    }

    public void DrawStartOfTurnHand()
    {
        SetHandState(HandState.START_TURN_DRAW_HAND);
        int handSize = 5; //TODO: get player hand size
        for (int i = 0; i < handSize; i++)
        {
            DrawFromPlayerDeck();
        }
    }

    public void ShuffleHandBackIntoDeck()
    {
        playerDeckDisplay.deck.ShuffleHandBackIn(cardsInHand);

        cardsInHand.Clear();

        while (this.transform.childCount > 0) {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }


    }

    public void SetPlayerDeck(Faction faction, Deck deck)
    {
        playerDeckDisplay.SetPlayerDeck(faction, deck);
    }

    public void BeginDragCard(CardDisplay cardDisplay)
    {
        // Debug.Log("Hand: beginDragCard");
        Card card = cardDisplay.displayCard;

        //check if enough essence to play card
        bool haveEnoughEssence = CanPlayCard(card);

        // Debug.Log("haveEnoughEssence = " + haveEnoughEssence);

        if(!haveEnoughEssence) {return;}

        // Debug.Log("Hand: beginDragCard enough essence");

        //set card possibilities
        battleManager.SetCardPossibilities(card);
    }

    public void EndDragCard()
    {
        battleManager.ClearPossibilities();
    }

    public void PlayCard(CardDisplay cardDisplay)
    {
        if(!CanPlayCard(cardDisplay.displayCard))
        {
            return;
        }

        //TODO handle event and agent cards

        if(cardDisplay.GetCardType() == CardType.ESSENCE)
        {       
            turnManager.PlayEssenceCard();     
            SetHandState(HandState.TARGET_SELECTION);

            EssenceCardDisplay ecDisplay = (EssenceCardDisplay) cardDisplay;
            
            //set card possibilities
            battleManager.SetCardPossibilities(ecDisplay.displayCard);

            //start essence action
            EssenceCard essenceCard = ecDisplay.PlayFromHand();
            


            cardPlaying = essenceCard;
            cardPlayingIndex = ecDisplay.positionInHand;

        }

        if(cardDisplay.GetCardType() == CardType.AGENT)
        {       
            turnManager.PlayAgentCard();     
            SetHandState(HandState.TARGET_SELECTION);

            AgentCardDisplay acDisplay = (AgentCardDisplay) cardDisplay;
            
            //set card possibilities
            battleManager.SetCardPossibilities(acDisplay.displayCard);

            //start essence action
            AgentCard agentCard = acDisplay.PlayFromHand();

            cardPlaying = agentCard;
            cardPlayingIndex = acDisplay.positionInHand;

        }

        
    }

    public void SelectTarget(BoardSpace boardSpace)
    {
        cardPlaying.SelectTarget(boardSpace);
    }

    public void SetHandState(HandState newHandState)
    {
        handState = newHandState;

        switch (handState)
        {
            case HandState.NONE: 
                break;
            case HandState.START_TURN_DRAW_HAND: 
                break;
            case HandState.START_TURN_DRAW_TIMELINE: 
                break;
            case HandState.CHOOSING: 
                break;
            case HandState.TARGET_SELECTION: 
                break;
            case HandState.ACTION_END:
                battleManager.ClearPossibilities();
                turnManager.SetVictoryPointUI();
                break;
            default:
                break;
        }
    }

    public void RemoveCardAfterPlaying()
    {
        //TODO: Implement animation effects

        Debug.Log(string.Format("# of cardsInHand: {0}, removing index: {1}", cardsInHand.Count, cardPlayingIndex));
    
        cardsInHand.RemoveAt(cardPlayingIndex);
        DestroyImmediate(this.transform.GetChild(cardPlayingIndex).gameObject);

        cardPlaying = null;

        SetHandState(HandState.CHOOSING);
    }

    public void UpdatePossibilities()
    {
        battleManager.ClearPossibilities();

        if(cardPlaying.data == null)
        {
            Debug.LogError("card.data playing is null");
        }

        //Debug.Log("UpdatePossibilities + " + cardPlaying.data.ToString());

        battleManager.SetCardPossibilities(cardPlaying);

    }
}
