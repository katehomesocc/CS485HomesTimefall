using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleStateMachine : MonoBehaviour
{
    static int PLAY_ESSENCE_CARD = 1;
    static int ROLL_AGENT_EFFECT = 1;
    static int PLAY_AGENT_CARD = 2;
    static int DISCARD_HAND_AND_DRAW = 2;
    static int REPLACE_TIMELINE_EVENT = 3;

    public static BattleStateMachine Instance;

    public enum GameState
    {
        StartOfGame,
        StartOfTurn,
        PlayerAction,
        EndOfTurn,
        EndOfCycle,
        EndOfGame
    }

    public GameState currentState;
    public FabricOfTime fabricOfTime;
    public GameObject[] turnOrderCovers = new GameObject[4];
    
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

    [Header("Current Turn")]
    public int currentTurn;
    public int currentTurnCycle = 1;
    public int totalTurns = 32;
    public Player currentPlayer;
    public Faction currentFaction;
    public int essenceCount;

    [Header("UI Elements")]
    public TMP_Text currentTurnText;
    public Button endTurnButton;

    [Header("Essence Count Textures")]
    public Texture ZERO_ESSENCE_TEXTURE;
    public Texture ONE_ESSENCE_TEXTURE;
    public Texture TWO_ESSENCE_TEXTURE;
    public Texture THREE_ESSENCE_TEXTURE;
    public RawImage essenceCountImage;

    [Header("Bots")]
    public BotCursor botCursor;

    private BoardManager boardManager;
    private BattleManager battleManager;
    private Hand hand;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        hand = Hand.Instance;
        boardManager = BoardManager.Instance;
        battleManager = BattleManager.Instance;

        currentState = GameState.StartOfGame;
    }

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

    public void StartGame()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (currentState != GameState.EndOfGame)
        {
            switch (currentState)
            {
                case GameState.StartOfGame:
                    yield return StartOfGame();
                    break;

                case GameState.StartOfTurn:
                    yield return StartOfTurn();
                    break;

                case GameState.PlayerAction:
                    yield return PlayerAction();
                    break;

                case GameState.EndOfTurn:
                    yield return EndOfTurn();
                    break;

                case GameState.EndOfCycle:
                    yield return EndOfCycle();
                    break;
            }
        }

        EndOfGame();
    }

    private IEnumerator StartOfGame()
    {
        Debug.Log("Starting the game.");
        currentTurn = 0;
        currentState = GameState.StartOfTurn;
        yield break;
    }

    private IEnumerator StartOfTurn()
    {
        currentTurn++;
        currentPlayer = GetPlayerForTurn();
        currentFaction = currentPlayer.faction;

        Debug.Log($"Starting turn {currentTurn}. {currentFaction} isbot = {currentPlayer.isBot}");

        boardManager.UnlockSpace(currentTurn, currentFaction);

        if(essenceCount == 0)
        {
            //set end turn button UI to highlighted as there are no further actions to be taken   
            endTurnButton.GetComponent<Image>().color = Color.white;
        }
        essenceCount = 3;

        UpdateUI();

        battleManager.SetSkybox(currentFaction);
        
        hand.SetPlayerDeck(currentFaction, currentPlayer.deck);
        hand.DrawStartOfTurnHand(currentPlayer);
        hand.TurnStartDrawFromTimelineDeck();

        if (currentPlayer.isBot)
        {
            StartCoroutine(ExecuteBotTurn());
        }
        else
        {
            currentState = GameState.PlayerAction;
        }

        yield break;
    }

    private IEnumerator PlayerAction()
    {
        Debug.Log($"Player {currentPlayer.name} is taking their turn.");
        //EnableEndTurnButton();

        // Wait for player to finish their actions
        while (currentState == GameState.PlayerAction)
        {
            yield return null;
        }
    }

    private IEnumerator EndOfTurn()
    {
        Debug.Log($"Ending turn {currentTurn}.");
        DisableEndTurnButton();

        botCursor.Disable();

        boardManager.ResolveEndOfTurnOnBoard();
        hand.ShuffleHandBackIntoDeck(currentPlayer);

        if (currentTurn % 4 == 0) // End of cycle
        {
            currentState = GameState.EndOfCycle;
            currentTurnCycle++;
        }
        else if (currentTurn >= totalTurns)
        {
            currentState = GameState.EndOfGame;
        }
        else
        {
            currentState = GameState.StartOfTurn;
        }

        yield break;
    }

    private IEnumerator EndOfCycle()
    {
        Debug.Log("Ending the cycle.");

        int cycleNumber = GetCycleNumber(currentTurn);
        int[] vpRoundArr = boardManager.CalculateVPForRound(cycleNumber);
        Faction roundWinner = GetWinner(vpRoundArr);
        Debug.Log($"cycle winner: {roundWinner}.");

        yield return fabricOfTime.PerformEndOfRoundUpdate(cycleNumber, roundWinner);

        currentState = currentTurn >= totalTurns ? GameState.EndOfGame : GameState.StartOfTurn;
        yield break;
    }

    private void EndOfGame()
    {
        Debug.Log("Game over. Calculating final scores.");

        int[] finalScores = boardManager.TotalVictoryPointsOnBoard();
        Faction winner = GetWinner(finalScores);

        Debug.Log($"Game winner: {winner}.");
        // Display final results
    }

    private Player GetPlayerForTurn()
    {
        int mod = currentTurn % 4;
        switch (mod)
        {
            case 1:
                int round = GetRoundFromTurn(currentTurn);
                roundNumText.text = string.Format("R{0}", round);
                stewardRoundVPText.text = "0";
                seekerRoundVPText.text = "0";
                sovereignRoundVPText.text = "0";
                weaverRoundVPText.text = "0";

                scoreboard.SetRoundHighlight(round);
                return battleManager.GetFactionPlayer(Faction.STEWARDS);
            case 2:
                return battleManager.GetFactionPlayer(Faction.SEEKERS);
            case 3:
                return battleManager.GetFactionPlayer(Faction.SOVEREIGNS);
            case 0: 
                return battleManager.GetFactionPlayer(Faction.WEAVERS);
            default: 
                return null;
        }
    }

    private int GetCycleNumber(int turn)
    {
        return Mathf.CeilToInt(turn / 4f);
    }

    private Faction GetWinner(int[] vpArr)
    {
        Debug.Log($"{vpArr}");
        int maxVP = Mathf.Max(vpArr);
        List<int> winners = new List<int>();
        for (int i = 0; i < vpArr.Length; i++)
        {
            if (vpArr[i] == maxVP)
            {
                winners.Add(i);
            }
        }

        if(winners.Count != 1)
        {
            return Faction.NONE;
        }

        switch(winners[0])
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

    private void UpdateUI()
    {
        // currentTurnText.text = $"Turn {currentTurn}: {currentFaction}";
        // roundNumText.text = $"Cycle {GetCycleNumber(currentTurn)}";

        SetEssenceTexture();

        SetTurnOrderCovers();

        DisableEndTurnButton();
    }

    private void SetEssenceTexture()
    {
        switch (essenceCount)
        {
            case 0: essenceCountImage.texture = ZERO_ESSENCE_TEXTURE; break;
            case 1: essenceCountImage.texture = ONE_ESSENCE_TEXTURE; break;
            case 2: essenceCountImage.texture = TWO_ESSENCE_TEXTURE; break;
            case 3: essenceCountImage.texture = THREE_ESSENCE_TEXTURE; break;
        }
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

    private void EnableEndTurnButton()
    {
        endTurnButton.interactable = true;
    }

    private void DisableEndTurnButton()
    {
        endTurnButton.interactable = false;
    }

    private IEnumerator ExecuteBotTurn()
    {
        currentState = GameState.PlayerAction;
        
        Debug.Log($"Bot {currentPlayer.name} is taking its turn.");

        //TODO: connect to bot AI logic

        yield return currentPlayer.botAI.StartTurn();

        Debug.Log($"Bot {currentPlayer.name} turn ended.");

        yield return new WaitForSeconds(0.1f);
        
        currentState = GameState.EndOfTurn;

        yield break;
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
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

    void ShowFOT()
    {
        fabricOfTime.ShowPanel();
    }

    void HideFOT()
    {
        fabricOfTime.HidePanel();
    }

    public bool HasEssenceToPlayCard(Card card)
    {
        if(card == null)
        { 
            Debug.LogError("card is null");
            return false;
        }

        CardType cardType = card.data.cardType;

        return HasEssenceToPlayCardType(cardType);
    }

    public bool HasEssenceToPlayCardType(CardType cardType)
    {
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
            case CardType.EVENT:

                if(essenceCount >= REPLACE_TIMELINE_EVENT)
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

    public bool HasEssenceToRollAgentEffect(AgentIcon agent)
    {
        if(agent.placedThisTurn) { return true;}
        if(essenceCount >= ROLL_AGENT_EFFECT)
        {
            return true;
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

    public bool RollAgentEffect(AgentIcon agent)
    {
        if(agent.placedThisTurn) { return true;}
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

    public void ReplaceTimelineEvent()
    {
        if(SpendEssence(REPLACE_TIMELINE_EVENT)) {return;}

        Debug.LogError("not enough essence");
        //do stuff
    }

    public void AttemptToSpendTest()
    {
        SpendEssence(-1);
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

    public void ResolveStartOfTurn()
    {
        hand.ResolveStartOfTurnInHand();
        boardManager.ResolveStartOfTurnOnBoard();
        EnableEndTurnButton();
    }

    public void KickoffEndTurn()
    {
        currentState = GameState.EndOfTurn;
    }

    public int GetEssenceCount()
    {
        return essenceCount;
    }

}
