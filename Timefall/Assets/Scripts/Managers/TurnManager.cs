using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    static int PLAY_ESSENCE_CARD = 1;
    static int ROLL_AGENT_EFFECT = 1;
    static int PLAY_AGENT_CARD = 2;
    static int DISCARD_HAND_AND_DRAW = 2;
    static int REPLACE_TIMELINE_EVENT = 3;
    public Button endTurnButton;

    BoardManager boardManager;

    Hand hand;
    public FabricOfTime fabricOfTime;

    public GameObject[] turnOrderCovers = new GameObject[4];

    [Header("Current Turn")]
    [SerializeField]
    private int currentTurn;
    [SerializeField]
    private Player currentPlayer;
    [SerializeField]
    private Faction currentFaction;
    [SerializeField]
    private int essenceCount;
    public TMP_Text currentTurnText;

    public TurnState turnState = TurnState.START_OF_GAME;

    [Header("Factions")]
    public Player seekerPlayer;
    public Player sovereignPlayer;
    public Player stewardPlayer;
    public Player weaverPlayer;

    [Header("Start Of Game")]
    public GameObject startOfGamePanel;
    public TMP_Text startOfGameText;

    public TMP_Text startOfGameCountdownText;

    [Header("ScoreBoard")]
    public Scoreboard scoreboard;
    public GameObject scoreboardPanel;
    public TMP_Text scoreboardLabelText;
    public TMP_Text scoreboardWinnerText;

    public List<Faction> roundWinners = new List<Faction>();

    [Header("Victory Points")]
    public TMP_Text roundNumText;
    public TMP_Text stewardVPText;
    public TMP_Text seekerVPText;
    public TMP_Text sovereignVPText;
    public TMP_Text weaverVPText;

    public TMP_Text stewardRoundVPText;
    public TMP_Text seekerRoundVPText;
    public TMP_Text sovereignRoundVPText;
    public TMP_Text weaverRoundVPText;

    [Header("Essence Count Textures")] 
    public Texture ZERO_ESSENCE_TEXTURE;
    public Texture ONE_ESSENCE_TEXTURE;
    public Texture TWO_ESSENCE_TEXTURE;
    public Texture THREE_ESSENCE_TEXTURE;
    public RawImage essenceCountImage;

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
        hand = Hand.Instance;
        boardManager = BoardManager.Instance;
        StartCoroutine(StartOfGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowScoreboard();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideScoreboard();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShowFOT();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            HideFOT();
        }
    }

    public void KickoffEndTurn()
    {
        if(turnState == TurnState.FACTION_TURN)
        {
            StartCoroutine(EndTurn());
        }
    }
    public IEnumerator EndTurn(){
        turnState = TurnState.END_OF_TURN;

        hand.ShuffleHandBackIntoDeck();

        int mod = currentTurn % 4;

        if(mod == 0) //end of turn round, do fabric of time calculation
        {
            Debug.Log("TM: Waiting for EndOfRound to finish...");
            yield return StartCoroutine(EndOfRound());
            Debug.Log("TM: ...Picking back up");
        }

        if(currentTurn == 32)
        {
            EndOfGame();
            yield break;
        }
        
        SetupNextFactionTurn();
    }

    public void EnableEndTurnButton()
    {
        endTurnButton.interactable = true;
    }

    public void DisableEndTurnButton()
    {
        endTurnButton.interactable = false;
    }

    public bool CanPlayCard(Card card)
    {
        if(card == null)
        { 
            Debug.LogError("card is null");
            return false;
        }

        CardType cardType = card.data.cardType;

        switch(cardType) 
        {
            case CardType.AGENT:
                if(essenceCount >= PLAY_AGENT_CARD)
                {
                    return true;
                }

                break;
            case CardType.ESSENCE:

                if(essenceCount >= PLAY_ESSENCE_CARD)
                {
                    return true;
                }

                break;
            default:
            //Error handling
                Debug.LogError("Invalid Card Type: " + cardType);
                break;
        }

        return false;
    }

    public bool PlayEssenceCard()
    {
        Debug.Log("Play essence card");
        SpendEssence(PLAY_ESSENCE_CARD);

        //do stuff

        return true;
    }

    public bool UseAgentEffect(AgentCard agentCard)
    {
        if(essenceCount >= ROLL_AGENT_EFFECT) {return false;}

        SpendEssence(ROLL_AGENT_EFFECT);

        //do stuff

        return true;
    }

    public bool PlayAgentCard()
    {
        if(essenceCount < PLAY_AGENT_CARD) {return false;}

        SpendEssence(PLAY_AGENT_CARD);

        //do stuff

        return true;
    }

    public bool DiscardHandAndDraw()
    {
        if(essenceCount >= DISCARD_HAND_AND_DRAW) {return false;}

        SpendEssence(DISCARD_HAND_AND_DRAW);

        //do stuff

        return true;
    }

    public bool ReplaceTimelineEvent(EventCard timelineEvent, EventCard replaceEvent)
    {
        if(essenceCount >= REPLACE_TIMELINE_EVENT) {return false;}

        SpendEssence(REPLACE_TIMELINE_EVENT);

        //do stuff

        return true;
    }

    public void AttemptToSpendTest()
    {
        SpendEssence(1);
    }

    public bool SpendEssence(int cost)
    {
        if(essenceCount < cost) {return false;}

        essenceCount = essenceCount - cost;
        SetEssenceTexture();

        //other stuff
        if(essenceCount == 0)
        {
            //set end turn button UI to highlighted as there are no further actions to be taken   
            endTurnButton.GetComponent<Image>().color = Color.yellow;
        }

        return true;
    }

    void SetEssenceTexture()
    {
        switch (essenceCount)
        {
            case 0:
                essenceCountImage.texture = ZERO_ESSENCE_TEXTURE;
                break;
            case 1:
                essenceCountImage.texture = ONE_ESSENCE_TEXTURE;
                break;
            case 2:
                essenceCountImage.texture = TWO_ESSENCE_TEXTURE;
                break;
            case 3:
                essenceCountImage.texture = THREE_ESSENCE_TEXTURE;
                break;
            default:
                Debug.LogError("shouldnt get here?");
                break;
        }
    }

    IEnumerator EndOfRound()
    {
            //calculate VP on sectioon played this turn
            //whoever wins gets wins this section of the fabric of time
            //if a tie occurs, Weavers hand size increased
        int roundNum = GetRoundFromTurn(currentTurn);
        int[] vpRoundArr = boardManager.CalculateVPForRound(roundNum);

        Faction roundWinner = GetWinner(vpRoundArr);
        roundWinners.Add(roundWinner);
        scoreboard.SetRoundWinner(roundNum, roundWinner);
        Debug.Log(string.Format("round[{0}] Winner: {1}", roundNum, roundWinner));

        yield return StartCoroutine (fabricOfTime.PerformEndOfRoundUpdate(roundNum, roundWinner));
    }

    void SetupNextFactionTurn()
    {
        turnState = TurnState.TURN_SETUP;
        currentTurn++;

        currentPlayer = GetPlayerForThisTurn();
        currentFaction = currentPlayer.faction;

        currentTurnText.text = string.Format("{0} | {1}", currentTurn, currentFaction);

        boardManager.UnlockSpace(currentTurn, currentFaction);

        hand.SetPlayerDeck(currentFaction, currentPlayer.deck);
        
        if(essenceCount == 0)
        {
            //set end turn button UI to highlighted as there are no further actions to be taken   
            endTurnButton.GetComponent<Image>().color = Color.white;
        }

        essenceCount = 3;
        
        SetEssenceTexture();

        SetTurnOrderCovers();

        DisableEndTurnButton();

        StartFactionTurn();
    }

    void StartFactionTurn()
    {
        turnState = TurnState.FACTION_TURN;
        // Debug.Log("Start faction turn");

        hand.DrawStartOfTurnHand();

        //draw from timeline deck and place on next available spot
        hand.DrawFromTimelineDeck();

        //spend essence

    }

    Player GetPlayerForThisTurn()
    {
        int mod = currentTurn % 4;
        int round = GetRoundFromTurn(currentTurn);

        switch(mod)
        {
            case 1:
                roundNumText.text = string.Format("R{0}", round);
                stewardRoundVPText.text = "0";
                seekerRoundVPText.text = "0";
                sovereignRoundVPText.text = "0";
                weaverRoundVPText.text = "0";

                scoreboard.SetRoundHighlight(round);
                return stewardPlayer;
            case 2:
                return seekerPlayer;
            case 3:
                return sovereignPlayer;
            case 0:
                return weaverPlayer;    
        }

        return null;
    }

    void EndOfGame()
    {
        // int[] vpArr = boardManager.TotalVictoryPointsOnBoard();
        // string result = string.Join(",", vpArr);
        // endOfGameVPText.text = result;
        // Debug.Log(result);

        scoreboard.SetRoundHighlight(9);

        int[] boardArr = boardManager.TotalVictoryPointsOnBoard();

        //add fabricOfTime VP 
        foreach (var faction in roundWinners)
        {
            switch (faction)
            {
                case Faction.STEWARDS:
                    boardArr[0] += 5;
                    break;
                case Faction.SEEKERS:
                    boardArr[1] += 5;
                    break;
                case Faction.SOVEREIGNS:
                    boardArr[2] += 5;
                    break;
                case Faction.WEAVERS:
                    boardArr[3] += 10;
                    break;
            }
        }

        scoreboardLabelText.text = "End of Game!";
        scoreboardWinnerText.text = GetWinner(boardArr).ToString();
        scoreboard.SetEndOfGameUI(boardArr);
        scoreboardPanel.SetActive(true);
    }

    IEnumerator StartOfGame()
    {
        turnState = TurnState.START_OF_GAME;
        //TODO: play audio?
        
        startOfGamePanel.SetActive(true);

        int startupTime = 0; //5; //commented for testing

        for (int i = startupTime; i > 0; i--)
        {
            startOfGameCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
            
        }

        startOfGameText.text = "";

        startOfGameCountdownText.text = "BATTLE";
        yield return new WaitForSeconds(1);

        startOfGamePanel.SetActive(false);

        SetupNextFactionTurn();

    }

        void SetTurnOrderCovers()
    {
        int mod = (currentTurn % 4);

        for (int i = 0; i < 4; i++)
        {
            if(i == mod)
            {
                turnOrderCovers[i].SetActive(false);
            } else
            {
                turnOrderCovers[i].SetActive(true);
            }
        }
        
    }

    public void SetVictoryPointUI()
    {
        int[] vpArr = boardManager.TotalVictoryPointsOnBoard();

        stewardVPText.text = GetVPText(vpArr[0]);
        seekerVPText.text = GetVPText(vpArr[1]);
        sovereignVPText.text = GetVPText(vpArr[2]);
        weaverVPText.text = GetVPText(vpArr[3]);

        //Set for turn round
        int roundNum = GetRoundFromTurn(currentTurn);
        int[] vpRoundArr = boardManager.CalculateVPForRound(roundNum);

        stewardRoundVPText.text = GetVPText(vpRoundArr[0]);
        seekerRoundVPText.text = GetVPText(vpRoundArr[1]);
        sovereignRoundVPText.text = GetVPText(vpRoundArr[2]);
        weaverRoundVPText.text = GetVPText(vpRoundArr[3]);

        scoreboard.UpdateRound(roundNum, vpRoundArr);
        scoreboard.UpdateBoard(vpArr);
    }

    string GetVPText(int vp)
    {
        string symbol = ""; 
        if(vp > 0) { symbol = "+";}
        else if (vp < 0) { symbol = "-";}

        return string.Format("{0}{1}", symbol, Mathf.Abs(vp));
    }

    int GetRoundFromTurn(int roundNum)
    {
        return (int) Mathf.Ceil(roundNum / 4f);
    }

    void ShowScoreboard()
    {
        scoreboardPanel.SetActive(true);
    }

    void HideScoreboard()
    {
        scoreboardPanel.SetActive(false);
    }

    Faction GetWinner(int[] boardArr)
    {
        int currMax = 0;
        int currMaxIndex = 0;
        bool tie = false;
        for (int i = 0; i < boardArr.Length; i++)
        {
            if(boardArr[i] == currMax)
            {
                tie = true;
            } else if (boardArr[i] > currMax)
            {
                currMax = boardArr[i];
                currMaxIndex = i;
                tie = false;
            }
        }

        Debug.Log(string.Format("Max idx:[{0}] val:[{1}] tie:[{2}]", currMaxIndex, currMax, tie));

        if(tie)
        {
            currMaxIndex = -1;
        }

        switch(currMaxIndex)
        {
            case 0:
                return Faction.STEWARDS;
            case 1:
                return Faction.SEEKERS;
            case 2:
                return Faction.SOVEREIGNS;
            case 3:
                return Faction.WEAVERS;    
            default:
                return Faction.NONE;
        }
  
    }

    void ShowFOT()
    {
        fabricOfTime.ShowPanel();
    }

    void HideFOT()
    {
        fabricOfTime.HidePanel();
    }

    
}