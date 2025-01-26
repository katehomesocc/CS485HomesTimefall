using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BotAI : MonoBehaviour
{
    public static float PAUSE = 0.25f;
    private BoardManager boardManager;
    private Hand hand;
    public enum BotState
    {
        AnalyzeBoard,
        ChooseAction,
        ExecuteAction,
        EndTurn
    }

    public BotState currentState;

    [Header("Player Info")]
    private int playerNumber;
    private Player botPlayer;

    [Header("BSM Info")]
    private BotCursor botCursor;
    private int currentTurnCycle;
    private List<BoardSpace> turnCycleSpaces = null;
    private List<BoardSpace> allSpaces = null;

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

    public IEnumerator ExecuteTurn()
    {

        if(hand.handState == HandState.START_TURN_DRAW_TIMELINE)
        {
            BattleManager.Instance.expandDisplay.AutoPlayInitialTimelineCard();
            yield return new WaitForSeconds(PAUSE);
        }

        //Debug.Break();

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

    private void AnalyzeBoard()
    {
        Debug.Log("Bot is analyzing the board.");

        // Get sorted board spaces for Turn Cycle and Whole Board
        turnCycleSpaces = BoardManager.Instance.GetBoardSpacesSortedByFactionVictoryPoints(botPlayer.faction, true);
        allSpaces = BoardManager.Instance.GetBoardSpacesSortedByFactionVictoryPoints(botPlayer.faction, false);

        Debug.Log($"Turn Cycle [{turnCycleSpaces.Count}] | All [{allSpaces.Count}]");

        Debug.Log("Prioritized spaces analyzed.");
        currentState = BotState.ChooseAction;
    }

    private void ChooseAction()
    {
        Debug.Log("Bot is choosing an action.");

        // Evaluate hand for priority actions based on Turn Cycle and Whole Board
        foreach (var cardDisplay in hand.displaysInHand)
        {
            // if (cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay, true)) // Turn Cycle
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }

            // if (cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay, true)) // Turn Cycle
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }
        }

        foreach (var cardDisplay in hand.displaysInHand)
        {
            // if (cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay, false)) // Whole Board
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }

            // if (cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay, false)) // Whole Board
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }
        }

        if(ChooseEssenceAction())
        {
            currentState = BotState.ExecuteAction;
            return;         
        }

        // If no actions are possible, end turn
        Debug.Log("No valid actions. Ending turn.");
        currentState = BotState.EndTurn;
    }

    private IEnumerator ExecuteAction()
    {
        // Debug.Log("Bot is executing an action.");
        yield return null;
    }

    private bool TryPlayEventCard(CardDisplay card, bool turnCycleOnly)
    {
        var targetSpaces = turnCycleOnly ? turnCycleSpaces : allSpaces;

        foreach (var space in targetSpaces)
        {
            if (space.isUnlocked && (space.isHole || space.hasEvent) && (space.eventCard.data.faction != botPlayer.faction))
            {
                StartCoroutine(ReplaceTimelineEvent(card, space));
                Debug.Log($"Played event card on the board (TurnCycleOnly: {turnCycleOnly}).");
                return true;
            }
        }

        return false;
    }

    private bool TryPlayAgentCard(CardDisplay card, bool turnCycleOnly)
    {
        var targetSpaces = turnCycleOnly ? turnCycleSpaces : allSpaces;

        foreach (var space in targetSpaces)
        {
            if (space.hasEvent && !space.hasAgent)
            {
                Debug.Log($"Deployed agent on the board (TurnCycleOnly: {turnCycleOnly}).");
                return true;
            }
        }

        return false;
    }

    private bool ChooseEssenceAction()
    {
        if(TryUseEssenceOfType(ActionType.Revive)) { return true; }

        if(TryUseEssenceOfType(ActionType.Swap)) { return true; }

        if(TryUseEssenceOfType(ActionType.Shield)) { return true; }

        if(TryUseEssenceOfType(ActionType.Paradox)) { return true; }

        if(TryUseEssenceOfType(ActionType.CosmicBlast)) { return true; }

        if(TryUseEssenceOfType(ActionType.Convert)) { return true; }

        if(TryUseEssenceOfType(ActionType.Channel)) { return true; }

        return false;
    }

    private bool TryUseEssenceOfType(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Channel:
                break;
            case ActionType.Convert:
                break;
            case ActionType.CosmicBlast:
                break;
            case ActionType.Paradox:
                break;
            case ActionType.Revive:
                break;
            case ActionType.Shield:
                break;
            case ActionType.Swap:
                if (currentTurnCycle == 1) { break; }
                return TryUseSwap();
            default:
                break;
        }

        return false;
    }

    private bool TryUseSwap()
    {
        foreach (CardDisplay cardDisplay in hand.displaysInHand)
        {
            // Ensure the card is a swap action type
            if (cardDisplay.GetActionType() != ActionType.Swap)
            {
                continue;
            }

            // If this swap card can't be played, none will work, so return false
            if (!cardDisplay.CanBePlayed(botPlayer))
            {
                return false;
            }

            // Cast cardDisplay to EssenceCardDisplay to access the actionRequest
            var essenceCardDisplay = (EssenceCardDisplay)cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            // Find the lowest VP card in the turn cycle that is targetable
            var lowestInCycle = turnCycleSpaces
                .Where(space => actionRequest.potentialBoardTargets.Contains(space))
                .OrderBy(space => space.eventCard.eventCardData.victoryPoints[playerNumber])
                .FirstOrDefault();

            // If no valid target in the turn cycle, return false
            if (lowestInCycle == null)
            {
                return false;
            }

            // Find the highest VP card out of the turn cycle that is targetable
            var highestOutOfCycle = allSpaces
                .Except(turnCycleSpaces)
                .Where(space => actionRequest.potentialBoardTargets.Contains(space))
                .OrderByDescending(space => space.eventCard.eventCardData.victoryPoints[playerNumber])
                .FirstOrDefault();

            // If no valid target outside the turn cycle, return false
            if (highestOutOfCycle == null)
            {
                return false;
            }

            // If the lowest VP card in the cycle has a greater than or equal VP than the highest out-of-cycle card, don't swap
            if (lowestInCycle.eventCard.eventCardData.victoryPoints[playerNumber] >=
                highestOutOfCycle.eventCard.eventCardData.victoryPoints[playerNumber])
            {
                return false;
            }

            // Execute the swap
            StartCoroutine(SwapEvents(cardDisplay, lowestInCycle, highestOutOfCycle));

            Debug.Log("Used SWAP essence card.");

            return true;
        }

        return false;
    }

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
    IEnumerator ReplaceTimelineEvent(CardDisplay eventToPlay, BoardSpace targetSpace)
    {
        yield return MoveCursor(eventToPlay.transform.position);

        eventToPlay.actionRequest.isBot = true;
        eventToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        
        hand.PlayCard(eventToPlay, true); // Play the event card, force = true
    }

    IEnumerator SwapEvents(CardDisplay handCardToPlay, BoardSpace target1, BoardSpace target2)
    {
        yield return MoveCursor(handCardToPlay.transform.position);

        handCardToPlay.actionRequest.isBot = true;
        handCardToPlay.actionRequest.activeBoardTargets.Add(target1);
        handCardToPlay.actionRequest.activeBoardTargets.Add(target2);

        Debug.Log("~~~~Swapping~~~~");
        Debug.Log($"[{target1.eventCard.data.cardName}] on space#{target1.spaceNumber} VP for {botPlayer.faction} = [{target1.eventCard.eventCardData.victoryPoints[playerNumber]}]");
        Debug.Log($"[{target2.eventCard.data.cardName}] on space#{target2.spaceNumber} VP for {botPlayer.faction} = [{target2.eventCard.eventCardData.victoryPoints[playerNumber]}]");

        Debug.Log("~~~~~~~~");
        
        hand.PlayCard(handCardToPlay, true); // Play the swap card, force = true
    }


}
