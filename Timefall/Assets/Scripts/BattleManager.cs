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
        
    }

    IEnumerator QueueFirstAutoplay()
    {
        yield return StartCoroutine(turnManager.StartOfGame());

        StartCoroutine(AutoplayRound());
    }

    IEnumerator AutoplayRound()
    {
        hand.AutoPlayTimelineCard();
        
        yield return turnManager.EndTurn();

        yield return new WaitForSeconds(0.1f);

        if(turn < autoplayUntilTurn)
        {
            turn++;
            StartCoroutine(AutoplayRound());
        }
        
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

    public void SetPossibleTargetHighlights(Card card)
    {
        boardManager.SetPossibleTargetHighlight(card);
        hand.SetPossibleTargetHighlight(card);
    }

    public void ClearPossibleTargetHighlights()
    {
        boardManager.ClearPossibleTargetHighlights();
        hand.ClearPossibleTargetHighlights();
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
}
