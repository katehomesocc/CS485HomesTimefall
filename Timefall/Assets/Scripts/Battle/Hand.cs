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

    public DiceRoller diceRoller;
    public Deck timelineDeck;
    public DeckDisplay playerDeckDisplay;
    public List<CardDisplay> targetsAvailable = new List<CardDisplay>();

        // [Header("Card Display Prefabs")]
    GameObject agentCardDisplay;
    GameObject essenceCardDisplay;
    GameObject eventCardDisplay;

    [Header("Buttons")]
    public Button drawPlayerButton;
    public Button shufflePlayerButton;
    public Button drawTimelineButton;
    public Button shuffleTimelineButton;

    [Header("Hand State")]
    public HandState handState = HandState.NONE;
    public Card cardPlaying;
    public int cardPlayingIndex;
    public ActionRequest currentActionRequest;

    public AgentIcon agentIconActing;

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

        agentCardDisplay = battleManager.agentCardDisplay;
        essenceCardDisplay = battleManager.essenceCardDisplay;
        eventCardDisplay = battleManager.eventCardDisplay;


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

    void ShuffleTimelineDeck()
    {
        ShuffleDeck(timelineDeck);
    }

    public void DrawFromTimelineDeck()
    {
        Card card = timelineDeck.Draw();

        if(card == null){ return;}

        CardDisplay display = battleManager.ExpandCardView(card, false);

        display.playState = CardPlayState.START_TURN_DRAW_TIMELINE;
        SetHandState(HandState.START_TURN_DRAW_TIMELINE);
    }

    public void PlayInitialTimelineCard(CardDisplay display)
    {
        if(display == null){ return;}

        boardManager.PlaceTimelineEventForTurn(display);
        turnManager.SetVictoryPointUI();
        turnManager.EnableEndTurnButton();

        turnManager.ResolveStartOfTurn();

        SetHandState(HandState.CHOOSING);
    }

    public void ResolveStartOfTurnInHand()
    {
        foreach (CardDisplay display in displaysInHand)
        {
            display.ResolveStartOfTurn();
        }
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
        // // Debug.Log("Hand: beginDragCard");
        // Card card = cardDisplay.displayCard;

        // //check if enough essence to play card
        // bool haveEnoughEssence = turnManager.HasEssenceToPlayCard(card);

        // // Debug.Log("haveEnoughEssence = " + haveEnoughEssence);

        // if(!haveEnoughEssence) {return;}

        // // Debug.Log("Hand: beginDragCard enough essence");

        // //set card possibilities
        // battleManager.SetPossibleTargetHighlights(card);
    }

    public void EndDragCard()
    {
        // battleManager.ClearPossibleTargetHighlights();
    }

    public void PlayCard(CardDisplay cardDisplay)
    {
        if(!turnManager.HasEssenceToPlayCard(cardDisplay.displayCard))
        {
            Debug.Log("not enough essence to play card");
            return;
        }

        currentActionRequest = cardDisplay.actionRequest;

        if(!cardDisplay.CanBePlayed(turnManager.GetCurrentPlayer()))
        {
            Debug.Log(currentActionRequest.ToString());
            Debug.Log("cannot play display");
            return;
        }

        Debug.Log("play card");

        //TODO handle event and agent cards

        if(cardDisplay.GetCardType() == CardType.ESSENCE)
        {       
            turnManager.PlayEssenceCard();     
            SetHandState(HandState.TARGET_SELECTION);

            EssenceCardDisplay ecDisplay = (EssenceCardDisplay) cardDisplay;

            currentActionRequest = ecDisplay.actionRequest;
            currentActionRequest.player = turnManager.GetCurrentPlayer();

            //start essence action
            EssenceCard essenceCard = ecDisplay.PlayFromHand();

            cardPlaying = essenceCard;
            cardPlayingIndex = ecDisplay.GetPositionInParent();

        }

        if(cardDisplay.GetCardType() == CardType.AGENT)
        {       
            turnManager.PlayAgentCard();     
            SetHandState(HandState.TARGET_SELECTION);

            AgentCardDisplay acDisplay = (AgentCardDisplay) cardDisplay;
            
            currentActionRequest = acDisplay.actionRequest;
            currentActionRequest.player = turnManager.GetCurrentPlayer();

            //start agent action
            AgentCard agentCard = acDisplay.PlayFromHand();

            cardPlaying = agentCard;
            cardPlayingIndex = acDisplay.GetPositionInParent();

        }
    }

    public void AttemptAgentAction(AgentIcon agentIcon)
    {
        if(!turnManager.HasEssenceToRollAgentEffect(agentIcon))
        {
            Debug.Log("not enough essence to play card");
            return;
        }
     
        cardPlaying = agentIcon.agentCard;
        agentIconActing = agentIcon;
        currentActionRequest = agentIcon.actionRequest;
        currentActionRequest.player = turnManager.GetCurrentPlayer();

        Debug.Log("attempt agent");

        SetHandState(HandState.DICE_ROLL);
        
        turnManager.RollAgentEffect(agentIcon);     

        agentIcon.AttemptAction();
    }

    public void StartAgentAction()
    {
        SetHandState(HandState.TARGET_SELECTION);
    }

    public void EndAgentAction()
    {
        //TODO: Implement animation effects
        agentIconActing = null;
    
        cardPlaying = null;

        SetHandState(HandState.CHOOSING);
    }

    public void SelectBoardTarget(BoardSpace boardSpace)
    {
        currentActionRequest.boardTarget = boardSpace;
        Debug.Log("Hand.SelectBoardTarget: " + currentActionRequest.ToString());
        cardPlaying.SelectBoardTarget(currentActionRequest);
    }

    public void SelectHandTarget(CardDisplay handDisplay)
    {
        currentActionRequest.handTarget = handDisplay;
        cardPlaying.SelectHandTarget(currentActionRequest);
    }

    public void SelectDiscardTarget(Card discardedCard)
    {
        currentActionRequest.discardedTarget = discardedCard;
        cardPlaying.SelectDiscardTarget(currentActionRequest);
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
                currentActionRequest = null;
                battleManager.ClearPossibleTargetHighlights(null);
                turnManager.SetVictoryPointUI();
                break;
            case HandState.DICE_ROLL:
                break;
            default:
                break;
        }
    }

    public void RemoveCardAfterPlaying(bool discard)
    {
        //TODO: Implement animation effects

        Debug.Log(string.Format("# of cardsInHand: {0}, removing index: {1}", cardsInHand.Count, cardPlayingIndex));
        
        cardsInHand.RemoveAt(cardPlayingIndex);
        displaysInHand.RemoveAt(cardPlayingIndex);

        DestroyImmediate(this.transform.GetChild(cardPlayingIndex).gameObject);

        if(discard){
            turnManager.GetCurrentPlayer().deck.Discard(cardPlaying);
        }
    
        cardPlaying = null;

        SetHandState(HandState.CHOOSING);
    }

    public void UpdatePossibilities(ActionRequest actionRequest)
    {
        Debug.Log("Hand.UpdatePossibilities: " + actionRequest.ToString());
        battleManager.ClearPossibleTargetHighlights(actionRequest);

        battleManager.SetPossibleTargetHighlights(cardPlaying, actionRequest);
    }

    public void SetPossibleTargetHighlight(Card card, ActionRequest actionRequest)
    {
            switch(card.GetCardType()) 
            {
                case CardType.AGENT:
                    // SetAgentPossibilities((AgentCard) card);
                    break;
                case CardType.ESSENCE:
                    SetEssencePossibilities((EssenceCard) card, actionRequest);
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

    public List<CardDisplay> GetPossibleTargets(Card card, ActionRequest actionRequest)
    {
        switch(card.GetCardType()) 
        {
            case CardType.AGENT:
                //return GetAgentPossibilities((AgentCard) card);
                break;
            case CardType.ESSENCE:
                return GetEssencePossibilities((EssenceCard) card, actionRequest);
            case CardType.EVENT:
                break;
            default:
            //Error handling
                Debug.LogError("Invalid Card Type: " + card.data.cardType);
                break;
        }
        return new List<CardDisplay>();
    }

    public List<CardDisplay> GetEssencePossibilities(EssenceCard essenceCard, ActionRequest actionRequest)
    {   
        actionRequest.potentialHandTargets = new List<CardDisplay>(displaysInHand);
        return essenceCard.GetTargatableHandDisplays(actionRequest);
    }

    void SetEssencePossibilities(EssenceCard essenceCard, ActionRequest actionRequest)
    {
        List<CardDisplay> targetable = GetEssencePossibilities(essenceCard, actionRequest);

        foreach (CardDisplay cardDisplay in targetable)
        {
            cardDisplay.HighlightOn();
            targetsAvailable.Add(cardDisplay);
            cardDisplay.isTargetable = true;
        }
    }

    public void AddCardToHand(Card card)
    {
        InstantiateCardInHand(card);
    }

        
}
