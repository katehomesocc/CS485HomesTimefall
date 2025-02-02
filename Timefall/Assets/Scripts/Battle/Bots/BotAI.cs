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

    [Header("BSM Info")]
    protected BotCursor botCursor;
    protected int currentTurnCycle;
    protected List<BoardSpace> turnCycleSpaces = null;
    protected List<BoardSpace> allSpaces = null;

    public void InitializeBot(Player player)
    {
        botPlayer = player;
        boardManager = BoardManager.Instance;
        hand = Hand.Instance;
        playerNumber = BattleManager.GetPlayerNumber(botPlayer.faction);
        currentState = BotState.AnalyzeBoard;
    }

    public IEnumerator StartTurn()
    {
        botCursor = BattleStateMachine.Instance.botCursor;
        botCursor.SetBot(this, botPlayer.faction);
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
        Action Enumerators
    */

    protected IEnumerator SwapEvents(CardDisplay handCardToPlay, BoardSpace target1, BoardSpace target2)
    {
        yield return MoveCursor(handCardToPlay.transform.position);

        handCardToPlay.actionRequest.isBot = true;
        handCardToPlay.actionRequest.activeBoardTargets.Add(target1);
        handCardToPlay.actionRequest.activeBoardTargets.Add(target2);

        hand.PlayCard(handCardToPlay, true); // Play the swap card, force = true
    }

    protected IEnumerator ReplaceTimelineEvent(CardDisplay eventToPlay, BoardSpace targetSpace)
    {
        yield return MoveCursor(eventToPlay.transform.position);

        eventToPlay.actionRequest.isBot = true;
        eventToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        
        hand.PlayCard(eventToPlay, true); // Play the event card, force = true
    }

    protected IEnumerator PlaceAgent(CardDisplay agentToPlay, BoardSpace targetSpace)
    {
        yield return MoveCursor(agentToPlay.transform.position);

        agentToPlay.actionRequest.isBot = true;
        agentToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        
        hand.PlayCard(agentToPlay, true);
    }

}
