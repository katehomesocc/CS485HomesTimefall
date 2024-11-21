using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    TurnManager turnManager;
    BoardManager boardManager;
    Hand hand;

    int turn = 1;

    public Deck timelineDeck;
    public DiscardPileDisplay inventory;
    public DiscardPileManager discardPileManager;

    public ExpandDisplay expandDisplay;

    [Header("Autoplay (Development Testing)")]
    public bool autoplay = false;
    public int autoplayUntilTurn = 32;
    public float autoplayWaitTime = 1.5f;

    [Header("Start Of Game")]
    public int startupTime = 5;
    public GameObject startOfGamePanel;
    public TMP_Text startOfGameText;

    public TMP_Text startOfGameCountdownText;

    [Header("Players")]
    public Player seekerPlayer;
    public Player sovereignPlayer;
    public Player stewardPlayer;
    public Player weaverPlayer;

    [Header("Skyboxes")]
    public Material seekerSkybox;
    public Material sovereignSkybox;
    public Material stewardSkybox;
    public Material weaverSkybox;

    [Header("Card Display Prefabs")]
    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;

    [Header("Colors")]
    public static Color COLOUR_SEEKERS = new Color(33f/255,197f/255,104f/255, 1f);
    public static Color COLOUR_SOVEREIGNS = new Color(255f/255,35f/255,147f/255, 1f);
    public static Color COLOUR_STEWARDS = new Color(24f/255,147f/255,248f/255, 1f);
    public static Color COLOUR_WEAVERS = new Color(97f/255,65f/255,172f/255, 1f);


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
        hand = Hand.Instance;

        StartCoroutine(StartOfGame());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            ShowDiscardPiles();
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            HideDiscardPiles();
        }
    }

    IEnumerator QueueFirstAutoplay()
    {
        yield return StartCoroutine(turnManager.StartOfGame());

        StartCoroutine(AutoplayRound());
    }

    IEnumerator AutoplayRound()
    {
        expandDisplay.AutoPlayInitialTimelineCard();
        
        yield return turnManager.EndTurn();

        yield return new WaitForSeconds(0.1f);

        if(turn < autoplayUntilTurn)
        {
            turn++;
            StartCoroutine(AutoplayRound());
        }
        
    }

    public void PlayInitialTimelineCard(CardDisplay display)
    {
        expandDisplay.PlayInitialTimelineCard(display);
    }

    IEnumerator StartOfGame()
    {
        startOfGamePanel.SetActive(true);

        for (int i = startupTime; i > 0; i--)
        {
            startOfGameCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
            
        }

        startOfGameText.text = "";

        startOfGameCountdownText.text = "BATTLE";
        yield return new WaitForSeconds(1);

        startOfGamePanel.SetActive(false);

        if(autoplay)
        {
            StartCoroutine(QueueFirstAutoplay());
        } else
        {
            turnManager.StartGame();
        }
    }

    public ActionRequest GetPotentialTargets(Card card, ActionRequest request)
    {
        if(request.doBoard)
        {
            request.potentialBoardTargets = boardManager.GetPossibleTargets(card, request);
        }

        if(request.doHand)
        {
            request.potentialHandTargets = hand.GetPossibleTargets(card, request);
        }

        if(request.doDiscard)
        {
            request.potentialDiscardedTargets = discardPileManager.GetPossibleTargets(card, request);
        }
           
        return request;
    }

    public void SetPossibleTargetHighlights(Card card, ActionRequest actionRequest)
    {
        if(actionRequest.doBoard)
        {
            boardManager.SetPossibleTargetHighlight(card, actionRequest);
        }

        if(actionRequest.doHand)
        {
            hand.SetPossibleTargetHighlight(card, actionRequest);
        }

        if(actionRequest.doDiscard)
        {
            discardPileManager.SetPossibleTargets(card, actionRequest);
        }
        
        
    }

    public void ClearPossibleTargetHighlights(ActionRequest actionRequest)
    {
        if(actionRequest != null)
        {
            actionRequest.ClearPossibleTargets();
        }
        boardManager.ClearPossibleTargetHighlights();
        hand.ClearPossibleTargetHighlights();
        discardPileManager.ClearAndClosePossibleTargets();
    }

    public Player GetFactionPlayer(Faction faction)
    {
        switch (faction)
        {
            case Faction.STEWARDS:
                return stewardPlayer;
            case Faction.SEEKERS:
                return seekerPlayer;
            case Faction.SOVEREIGNS:
                return sovereignPlayer;   
            case Faction.WEAVERS:
                return weaverPlayer;    
        }

        return null;
    }

    Material GetFactionSkybox(Faction faction)
    {
        switch (faction)
        {
            case Faction.STEWARDS:
                return stewardSkybox;
            case Faction.SEEKERS:
                return seekerSkybox;
            case Faction.SOVEREIGNS:
                return sovereignSkybox; 
            case Faction.WEAVERS:
                return weaverSkybox;  
        }

        return null;
    }

    public static Color GetFactionColor(Faction faction)
    {
        switch(faction) 
        {
            case Faction.WEAVERS:
                return COLOUR_WEAVERS;
            case Faction.SEEKERS:
                return COLOUR_SEEKERS;
            case Faction.SOVEREIGNS:
                return COLOUR_SOVEREIGNS;
            case Faction.STEWARDS:
                return COLOUR_STEWARDS;
        }

        return Color.black;
    }

    public void DiscardToDeck(Card card, Faction faction)
    {
        if(faction == Faction.NONE)
        {
            timelineDeck.Discard(card);
            return;
        }

        Player player = GetFactionPlayer(faction);
        player.deck.Discard(card);
        
    }

    public void SetSkybox(Faction faction)
    {
        RenderSettings.skybox = GetFactionSkybox(faction);
    }

    public void SetAndShowInventory(List<Card> cardsToDisplay)
    {
        inventory.SetInventory(cardsToDisplay);
        inventory.OpenInventory();
    }

    public void ClearAndCloseInventory()
    {
        inventory.ClearInventory();
        inventory.CloseInventory();
    }

    void ShowDiscardPiles()
    {
        discardPileManager.ShowDisplays();
    }

    void HideDiscardPiles()
    {
        discardPileManager.HideDisplays();
    }

    public void ConvertAgent(AgentCard agentToConvert, Faction newFaction)
    {
        Faction oldFaction = agentToConvert.GetFaction();
        Player oldOwner = GetFactionPlayer(oldFaction);
        
        oldOwner.deck.discardPile.Remove(agentToConvert);

        Player newOwner = GetFactionPlayer(newFaction);
        newOwner.ConvertAgent(agentToConvert);
    }

    public CardDisplay ExpandCardView(Card card, bool hoverClear)
    {
        return expandDisplay.ExpandCardView(card, hoverClear);
    }

    public void CloseExpandCardView()
    {
        expandDisplay.CloseExpandCardView();
    }
}
