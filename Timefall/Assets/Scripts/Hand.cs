using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public static Hand Instance;
    public static string LOCATION = "HAND";

    public RectTransform  handPanel;

    public List<Card> cardsInHand = new List<Card>();
    public List<CardDisplay> displaysInHand = new List<CardDisplay>();
    BoardManager boardManager;
    TurnManager turnManager;
    BattleManager battleManager;
    public Deck timelineDeck;
    public DeckDisplay playerDeckDisplay;
    public List<CardDisplay> targetsAvailable = new List<CardDisplay>();

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
        turnManager = TurnManager.Instance;
        boardManager = BoardManager.Instance;
        battleManager = BattleManager.Instance;

        timelineDeck = battleManager.timelineDeck;

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
        Card card = DrawFromDeck(playerDeckDisplay.deck);

        InstantiateCardInHand(card);

    }

    void ShufflePlayerDeck()
    {
        ShuffleDeck(playerDeckDisplay.deck);
    }

    Card DrawFromDeck(Deck targetDeck){
        return targetDeck.Draw();
	}

    CardDisplay InstantiateCardInHand(Card card)
    {
        CardType cardType = card.data.cardType;

        CardDisplay returnDisplay;
        
        switch(cardType) 
        {
            case CardType.AGENT:
                returnDisplay = InstantiateAgentInHand((AgentCard) card);
                break;
            case CardType.ESSENCE:
                returnDisplay = InstantiateEssenceInHand((EssenceCard) card);
                break;
            case CardType.EVENT:
                returnDisplay = InstantiateEventInHand((EventCard) card);

                break;
            default:
            //Error handling
                Debug.LogError("Invalid CardData Type: " + cardType);
                return null;
        }

        cardsInHand.Add(card);
        return returnDisplay;
    }

    CardDisplay InstantiateAgentInHand(AgentCard agentCard)
    {
        GameObject obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

        if(agentDC == null){return null;} 

        agentDC.SetCard(agentCard);
        agentDC.InstantiateInHand(transform);
        agentDC.positionInHand = displaysInHand.Count;

        displaysInHand.Add(agentDC);
        
        return agentDC;
    }

    CardDisplay InstantiateEssenceInHand(EssenceCard essenceCard)
    {
        GameObject obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

        if(essenceDC == null){return null;} 

        essenceDC.SetCard(essenceCard);
        essenceDC.InstantiateInHand(transform);
        essenceDC.positionInHand = displaysInHand.Count;
        displaysInHand.Add(essenceDC);

        return essenceDC;
    }

    CardDisplay InstantiateEventInHand(EventCard eventCard)
    {
        GameObject obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

        if(eventDC == null){return null;} 

        eventDC.SetCard(eventCard);
        eventDC.InstantiateInHand(transform);
        eventDC.positionInHand = displaysInHand.Count;
        displaysInHand.Add(eventDC);

        return eventDC;
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

        staticCards.RemoveAt(0);

        //handle pre turn card effects
        PreTurnCardEffects();

        SetHandState(HandState.CHOOSING);
    }

    public void AutoPlayTimelineCard()
    {
        if(staticCards.Count == 1)
        {
            PlayInitialTimelineCard(staticCards[0]);
        }
    }

    void PreTurnCardEffects()
    {
        ResolveChannelEffects();
    }

    void ResolveChannelEffects()
    {
        foreach (CardDisplay display in displaysInHand)
        {
            if(display.displayCard.channeling)
            {
                display.RemoveChannelEffect();
            }
        }
    }

    public bool CanPlayCard(Card card)
    {
        return turnManager.CanPlayCard(card) && card.CanBePlayed();
    }

    public void DrawStartOfTurnHand(Player player)
    {
        SetHandState(HandState.START_TURN_DRAW_HAND);
        
        int drawSize = player.handSize - player.channelList.Count;

        DrawChanneledCards(player);

        for (int i = 0; i < drawSize; i++)
        {
            DrawFromPlayerDeck();
        }
    }

    void DrawChanneledCards(Player player)
    {
        //TODO: animation?
        List<Card> channelList = player.channelList;

        foreach (Card channelCard in channelList)
        {
            CardDisplay channelDisplay = InstantiateCardInHand(channelCard);
            channelDisplay.ApplyChannelEffect();
        }

        player.channelList.Clear();
    }

    void HandleChanneledCardsAtShuffle(Player player)
    {
        //TODO: animation?
        List<Card> channelList = player.channelList;

        foreach (Card channelCard in channelList)
        {
            cardsInHand.Remove(channelCard);
        }

    }

    public void ShuffleHandBackIntoDeck(Player player)
    {
        HandleChanneledCardsAtShuffle(player);

        playerDeckDisplay.deck.ShuffleHandBackIn(cardsInHand);

        cardsInHand.Clear();
        displaysInHand.Clear();

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
        battleManager.SetPossibleTargetHighlights(card);
    }

    public void EndDragCard()
    {
        battleManager.ClearPossibleTargetHighlights();
    }

    public void PlayCard(CardDisplay cardDisplay)
    {
        if(!CanPlayCard(cardDisplay.displayCard))
        {
            Debug.Log("cannot play card");
            return;
        }

        Debug.Log("play card");

        //TODO handle event and agent cards

        if(cardDisplay.GetCardType() == CardType.ESSENCE)
        {       
            turnManager.PlayEssenceCard();     
            SetHandState(HandState.TARGET_SELECTION);

            EssenceCardDisplay ecDisplay = (EssenceCardDisplay) cardDisplay;
            
            //set card possibilities
            battleManager.SetPossibleTargetHighlights(ecDisplay.displayCard);

            //start essence action
            EssenceCard essenceCard = ecDisplay.PlayFromHand();

            cardPlaying = essenceCard;
            cardPlayingIndex = ecDisplay.GetPositionInHand();

        }

        if(cardDisplay.GetCardType() == CardType.AGENT)
        {       
            turnManager.PlayAgentCard();     
            SetHandState(HandState.TARGET_SELECTION);

            AgentCardDisplay acDisplay = (AgentCardDisplay) cardDisplay;
            
            //set card possibilities
            battleManager.SetPossibleTargetHighlights(acDisplay.displayCard);

            //start essence action
            AgentCard agentCard = acDisplay.PlayFromHand();

            cardPlaying = agentCard;
            cardPlayingIndex = acDisplay.GetPositionInHand();

        }
    }

    public void SelectTarget(BoardSpace boardSpace)
    {
        cardPlaying.SelectTarget(boardSpace);
    }

    public void SelectTarget(CardDisplay handDisplay)
    {
        cardPlaying.SelectTarget(handDisplay);
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
                battleManager.ClearPossibleTargetHighlights();
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
        displaysInHand.RemoveAt(cardPlayingIndex);

        DestroyImmediate(this.transform.GetChild(cardPlayingIndex).gameObject);

        cardPlaying = null;

        SetHandState(HandState.CHOOSING);
    }

    public void UpdatePossibilities()
    {
        battleManager.ClearPossibleTargetHighlights();

        if(cardPlaying.data == null)
        {
            Debug.LogError("card.data playing is null");
        }

        //Debug.Log("UpdatePossibilities + " + cardPlaying.data.ToString());

        battleManager.SetPossibleTargetHighlights(cardPlaying);

    }

    public void SetPossibleTargetHighlight(Card card)
    {
            switch(card.GetCardType()) 
            {
                case CardType.AGENT:
                    // SetAgentPossibilities((AgentCard) card);
                    break;
                case CardType.ESSENCE:
                    SetEssencePossibilities((EssenceCard) card);
                    break;
                case CardType.EVENT:


                    break;
                default:
                //Error handling
                    Debug.LogError("Invalid Card Type: " + card.data.cardType);
                    return;
            }

    }

    public void ClearPossibleTargetHighlights()
    {
        foreach (CardDisplay cardDisplay in targetsAvailable)
        {
            cardDisplay.HighlightOff();
            cardDisplay.isTargetable = false;
        }

        targetsAvailable.Clear();
    }

    public List<CardDisplay> GetPossibleTargets(Card card)
    {
        switch(card.GetCardType()) 
        {
            case CardType.AGENT:
                //return GetAgentPossibilities((AgentCard) card);
                break;
            case CardType.ESSENCE:
                return GetEssencePossibilities((EssenceCard) card);
            case CardType.EVENT:
                break;
            default:
            //Error handling
                Debug.LogError("Invalid Card Type: " + card.data.cardType);
                break;
        }
        return null;
    }

    public List<CardDisplay> GetEssencePossibilities(EssenceCard essenceCard)
    {   
        return essenceCard.GetTargatableHandDisplays(displaysInHand, essenceCard.handTargets);
    }

    void SetEssencePossibilities(EssenceCard essenceCard)
    {
        List<CardDisplay> targetable = GetEssencePossibilities(essenceCard);

        foreach (CardDisplay cardDisplay in targetable)
        {
            cardDisplay.HighlightOn();
            targetsAvailable.Add(cardDisplay);
            cardDisplay.isTargetable = true;
        }
    }

    
}
