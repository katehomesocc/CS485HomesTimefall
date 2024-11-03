using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TurnManager : MonoBehaviour
{
    static int PLAY_ESSENCE_CARD, ROLL_AGENT_EFFECT = 1;
    static int PLAY_AGENT_CARD, DISCARD_HAND_AND_DRAW = 2;
    static int REPLACE_TIMELINE_EVENT = 3;
    public Button endTurnButton;

    public BoardManager boardManager;

    public Hand hand;

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
    public TMP_Text essenceText;
    public TMP_Text currentTurnText;

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

    public List<Faction> cycleWinners = new List<Faction>();

    [Header("Victory Points")]
    public TMP_Text cycleNumText;
    public TMP_Text stewardVPText;
    public TMP_Text seekerVPText;
    public TMP_Text sovereignVPText;
    public TMP_Text weaverVPText;

    public TMP_Text stewardCycleVPText;
    public TMP_Text seekerCycleVPText;
    public TMP_Text sovereignCycleVPText;
    public TMP_Text weaverCycleVPText;

    // Start is called before the first frame update
    void Start()
    {
        currentTurn = 0;
        essenceCount = 3;

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
    }

    
    public void EndTurn(){
        hand.ShuffleHandBackIntoDeck();
        EndOfTurnCycle();
    }

    public bool CanPlayCard(Card card)
    {
        if(card == null){ return false;}

        CardType cardType = card.cardType;

        switch(cardType) 
        {
            case CardType.AGENT:
                if(essenceCount < PLAY_AGENT_CARD)
                {
                    return true;
                }

                break;
            case CardType.ESSENCE:

                if(essenceCount < PLAY_ESSENCE_CARD)
                {
                    return true;
                }

                break;
            default:
            //Error handling
                Debug.Log ("Invalid Card Type: " + cardType);
                break;
        }

        return false;
    }

    public bool PlayEssenceCard(EssenceCard essenceCard)
    {
        if(essenceCount < PLAY_ESSENCE_CARD) {return false;}

        //do stuff

        return true;
    }

    public bool UseAgentEffect(AgentCard agentCard)
    {
        if(essenceCount < ROLL_AGENT_EFFECT) {return false;}

        //do stuff

        return true;
    }

    public bool PlayAgentCard(AgentCard agentCard)
    {
        if(essenceCount < PLAY_AGENT_CARD) {return false;}

        //do stuff

        return true;
    }

    public bool DiscardHandAndDraw()
    {
        if(essenceCount < DISCARD_HAND_AND_DRAW) {return false;}

        //do stuff

        return true;
    }

    public bool ReplaceTimelineEvent(EventCard timelineEvent, EventCard replaceEvent)
    {
        if(essenceCount < REPLACE_TIMELINE_EVENT) {return false;}

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

        essenceCount -= cost;
        essenceText.text = string.Format("{0}§", essenceCount);

        //other stuff
        if(essenceCount == 0)
        {
            //set end turn button UI to highlighted as there are no further actions to be taken   
            endTurnButton.GetComponent<Image>().color = Color.yellow;
        }

        return true;
    }

    void EndOfTurnCycle()
    {
        int mod = currentTurn % 4;

        if(mod == 0) //end of turny cycle, do fabric of time calculation
        {
             //calculate VP on sectioon played this turn
                //whoever wins gets wins this section of the fabric of time
                //if a tie occurs, Weavers hand size increased
            int cycleNum = GetCycleNumFromRoundNum(currentTurn);
            int[] vpCycleArr = boardManager.CalculateVPForTurnCycle(cycleNum);

            Faction cycleWinner = GetWinner(vpCycleArr);
            cycleWinners.Add(cycleWinner);
            scoreboard.SetCycleWinner(cycleNum, cycleWinner);
            Debug.Log(string.Format("Cycle[{0}] Winner: {1}", cycleNum, cycleWinner));
        }

        if(currentTurn == 32)
        {
            EndOfGame();
            return;
        }
        
        SetupNextFactionTurn();
    }

    void SetupNextFactionTurn()
    {
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
        essenceText.text = string.Format("{0}§", essenceCount);

        SetTurnOrderCovers();

        StartFactionTurn();
    }

    void StartFactionTurn()
    {
        Debug.Log("Start faction turn");

        hand.DrawStartOfTurnHand();

        //draw from timeline deck and place on next available spot
        hand.DrawFromTimelineDeck();

        //spend essence

    }

    Player GetPlayerForThisTurn()
    {
        int mod = currentTurn % 4;
        int cycle = GetCycleNumFromRoundNum(currentTurn);

        switch(mod)
        {
            case 1:
                cycleNumText.text = string.Format("R{0}", cycle);
                stewardCycleVPText.text = "0";
                seekerCycleVPText.text = "0";
                sovereignCycleVPText.text = "0";
                weaverCycleVPText.text = "0";

                scoreboard.SetCycleHighlight(cycle);
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

        scoreboard.SetCycleHighlight(9);

        int[] boardArr = boardManager.TotalVictoryPointsOnBoard();

        //add fabricOfTime VP 
        foreach (var faction in cycleWinners)
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

        //Set for turn cycle
        int cycleNum = GetCycleNumFromRoundNum(currentTurn);
        int[] vpCycleArr = boardManager.CalculateVPForTurnCycle(cycleNum);

        stewardCycleVPText.text = GetVPText(vpCycleArr[0]);
        seekerCycleVPText.text = GetVPText(vpCycleArr[1]);
        sovereignCycleVPText.text = GetVPText(vpCycleArr[2]);
        weaverCycleVPText.text = GetVPText(vpCycleArr[3]);

        scoreboard.UpdateRound(cycleNum, vpCycleArr);
        scoreboard.UpdateBoard(vpArr);
    }

    string GetVPText(int vp)
    {
        string symbol = ""; 
        if(vp > 0) { symbol = "+";}
        else if (vp < 0) { symbol = "-";}

        return string.Format("{0}{1}", symbol, Mathf.Abs(vp));
    }

    int GetCycleNumFromRoundNum(int roundNum)
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



    
}
