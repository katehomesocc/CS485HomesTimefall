using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class BotAI : MonoBehaviour
{
    public static float PAUSE = 0.25f;
    protected BoardManager boardManager;
    protected Hand hand;

    public enum BotState
    {
        AnalyzeBoard,
        ChooseAction,
        ExecuteAction,
        EndTurn
    }

    public BotState currentState;

    [Header("Player Info")]
    protected int playerNumber;
    protected Player botPlayer;
    public Faction faction { get; private set; }

    [Header("BSM Info")]
    protected BotCursor botCursor;
    protected int currentTurnCycle;
    protected List<BoardSpace> turnCycleSpaces = null;
    protected List<BoardSpace> allSpaces = null;

    protected bool agentEnoughEssence = true;
    protected bool eventEnoughEssence = true;
    protected bool essenceEnoughEssence = true;

    public void InitializeBot(Player player)
    {
        botPlayer = player;
        faction = player.faction;
        boardManager = BoardManager.Instance;
        hand = Hand.Instance;
        playerNumber = BattleManager.GetPlayerNumber(faction);
        currentState = BotState.AnalyzeBoard;
    }

    public IEnumerator StartTurn()
    {
        botCursor = BattleStateMachine.Instance.botCursor;
        botCursor.SetBot(this, faction);
        botCursor.Enable();

        currentTurnCycle = BattleStateMachine.Instance.currentTurnCycle;
        currentState = BotState.AnalyzeBoard;

        yield return ExecuteTurn();
    }

    protected IEnumerator ExecuteTurn()
    {
        if (hand.handState == HandState.START_TURN_DRAW_TIMELINE)
        {
            BattleManager.Instance.expandDisplay.AutoPlayInitialTimelineCard();
            yield return new WaitForSeconds(PAUSE);
        }

        while (currentState != BotState.EndTurn)
        {
            switch (currentState)
            {
                case BotState.AnalyzeBoard:
                    AnalyzeBoard();
                    break;
                case BotState.ChooseAction:
                    ChooseAction();
                    break;
                case BotState.ExecuteAction:
                    yield return ExecuteAction();
                    break;
            }
            yield return null;
        }

        botCursor.Enable();
        yield return new WaitForSeconds(PAUSE);
        yield return MoveCursor(BattleStateMachine.Instance.endTurnButton.transform.position);
        yield return new WaitForSeconds(PAUSE);
    }

    public void EndAction()
    {
        turnCycleSpaces = null;
        allSpaces = null;
        currentState = BattleStateMachine.Instance.GetEssenceCount() > 0 ? BotState.AnalyzeBoard : BotState.EndTurn;
    }

    protected abstract void AnalyzeBoard();
    protected abstract void ChooseAction();
    protected abstract IEnumerator ExecuteAction();

    public IEnumerator MoveCursor(Vector3 worldPos)
    {
        yield return new WaitForSeconds(PAUSE);
        yield return botCursor.MoveToPosition(worldPos);
        AudioManager.Instance.Play(BattleManager.Instance.BOT_CLICK_SOUND);
        yield return new WaitForSeconds(PAUSE);
    }

    /*
        Try To Play Bools
    */
    protected bool TryPlayAgentCard(CardDisplay card, bool turnCycleOnly)
    {
        List<BoardSpace> targetSpaces = turnCycleOnly ? turnCycleSpaces : allSpaces;

        foreach (BoardSpace space in targetSpaces)
        {
            if (space.hasEvent && !space.hasAgent)
            {
                StartCoroutine(PlaceAgent(card, space));
                Debug.Log($"Deployed agent on the board (TurnCycleOnly: {turnCycleOnly}).");
                return true;
            }
        }

        return false;
    }

    private bool TryPlayEventCard(CardDisplay card, bool turnCycleOnly)
    {
        List<BoardSpace> targetSpaces = turnCycleOnly ? turnCycleSpaces : allSpaces;

        foreach (BoardSpace space in targetSpaces)
        {
            if (space.isUnlocked && (space.isHole || space.hasEvent) && (space.eventCard.data.faction != faction))
            {
                StartCoroutine(ReplaceTimelineEvent(card, space));
                Debug.Log($"Played event card on the board (TurnCycleOnly: {turnCycleOnly}).");
                return true;
            }
        }

        return false;
    }

    /*
        Action Enumerators
    */

    protected IEnumerator Channel(CardDisplay handDisplay, CardDisplay displayToChannel)
    {
        yield return MoveCursor(handDisplay.transform.position);

        handDisplay.actionRequest.isBot = true;
        handDisplay.actionRequest.handTarget = displayToChannel;
        
        hand.PlayCard(handDisplay, true); // Channel the card, force = true
    }

    protected IEnumerator ConvertAgent(CardDisplay handDisplay, Card agentToConvert)
    {
        yield return MoveCursor(handDisplay.transform.position);

        handDisplay.actionRequest.isBot = true;
        handDisplay.actionRequest.discardedTarget = agentToConvert;
        
        hand.PlayCard(handDisplay, true); // Convert the agent card, force = true
    }

    protected IEnumerator CosmicBlast(CardDisplay eventToPlay, BoardSpace targetSpace)
    {
        yield return MoveCursor(eventToPlay.transform.position);

        eventToPlay.actionRequest.isBot = true;
        eventToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        
        hand.PlayCard(eventToPlay, true); // Play the paradox card, force = true
    }

    protected IEnumerator Shield(CardDisplay handCardToPlay, BoardSpace targetSpace)
    {
        yield return MoveCursor(handCardToPlay.transform.position);

        handCardToPlay.actionRequest.isBot = true;
        handCardToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        
        hand.PlayCard(handCardToPlay, true); // Play the shield card, force = true
    }


    protected IEnumerator SwapEvents(CardDisplay handCardToPlay, BoardSpace target1, BoardSpace target2)
    {
        yield return MoveCursor(handCardToPlay.transform.position);

        handCardToPlay.actionRequest.isBot = true;
        handCardToPlay.actionRequest.activeBoardTargets.Add(target1);
        handCardToPlay.actionRequest.activeBoardTargets.Add(target2);

        hand.PlayCard(handCardToPlay, true); // Play the swap card, force = true
    }
    protected IEnumerator Paradox(CardDisplay eventToPlay, BoardSpace targetSpace)
    {
        yield return MoveCursor(eventToPlay.transform.position);

        eventToPlay.actionRequest.isBot = true;
        eventToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        
        hand.PlayCard(eventToPlay, true); // Play the paradox card, force = true
    }

    protected IEnumerator PlaceAgent(CardDisplay agentToPlay, BoardSpace targetSpace)
    {
        yield return MoveCursor(agentToPlay.transform.position);

        agentToPlay.actionRequest.isBot = true;
        agentToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        
        hand.PlayCard(agentToPlay, true);
    }

    protected IEnumerator ReplaceTimelineEvent(CardDisplay eventToPlay, BoardSpace targetSpace)
    {
        yield return MoveCursor(eventToPlay.transform.position);

        eventToPlay.actionRequest.isBot = true;
        eventToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        
        hand.PlayCard(eventToPlay, true); // Play the event card, force = true
    }

    protected IEnumerator ReviveAgent(CardDisplay handDisplay, Card agentToRevive)
    {
        yield return MoveCursor(handDisplay.transform.position);

        handDisplay.actionRequest.isBot = true;
        handDisplay.actionRequest.discardedTarget = agentToRevive;
        
        hand.PlayCard(handDisplay, true); // Revive the agent card, force = true
    }

}
