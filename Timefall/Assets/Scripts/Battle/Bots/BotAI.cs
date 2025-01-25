using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BotAI 
{
    BattleStateMachine battleStateMachine;
    public enum BotState
    {
        AnalyzeBoard,
        ChooseAction,
        ExecuteAction,
        EndTurn
    }

    public BotState currentState;
    private Player botPlayer;
    private BoardManager boardManager;
    private Hand hand;

    private List<BoardSpace> turnCycleSpaces = null;
    private List<BoardSpace> allSpaces = null;

    public void InitializeBot(Player player)
    {
        botPlayer = player;
        boardManager = BoardManager.Instance;
        hand = Hand.Instance;
        currentState = BotState.AnalyzeBoard;
    }

    public IEnumerator StartTurn()
    {
        currentState = BotState.AnalyzeBoard;

        yield return ExecuteTurn();
    }

    public IEnumerator ExecuteTurn()
    {

        if(hand.handState == HandState.START_TURN_DRAW_TIMELINE)
        {
            BattleManager.Instance.expandDisplay.AutoPlayInitialTimelineCard();
            yield return new WaitForSeconds(0.1f);
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

        yield return new WaitForSeconds(0.1f);
    }

    public void EndAction()
    {
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
            if (cardDisplay.displayCard.data.cardType == CardType.EVENT && TryPlayEventCard(cardDisplay, true)) // Turn Cycle
            {
                currentState = BotState.ExecuteAction;
                return;
            }

            // if (cardDisplay.displayCard.data.cardType == CardType.AGENT && TryPlayAgentCard(cardDisplay, true)) // Turn Cycle
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }
        }

        foreach (var cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.displayCard.data.cardType == CardType.EVENT && TryPlayEventCard(cardDisplay, false)) // Whole Board
            {
                currentState = BotState.ExecuteAction;
                return;
            }

            // if (cardDisplay.displayCard.data.cardType == CardType.AGENT && TryPlayAgentCard(cardDisplay, false)) // Whole Board
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }

            // if (cardDisplay.displayCard.data.cardType == CardType.ESSENCE && TryUseEssenceCard(cardDisplay))
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }
        }

        // If no actions are possible, end turn
        Debug.Log("No valid actions. Ending turn.");
        currentState = BotState.EndTurn;
    }

    private IEnumerator ExecuteAction()
    {
        Debug.Log("Bot is executing an action.");
        yield return null;
    }

    private bool TryPlayEventCard(CardDisplay card, bool turnCycleOnly)
    {
        var targetSpaces = turnCycleOnly ? turnCycleSpaces : allSpaces;

        foreach (var space in targetSpaces)
        {
            if (space.isUnlocked && (space.isHole || space.hasEvent) && (space.eventCard.data.faction != botPlayer.faction))
            {
                ReplaceTimelineEvent(card, space);
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

    private bool TryUseEssenceCard(CardDisplay card)
    {
        card.actionRequest.isBot = true;
        // Use essence cards for shielding or disruption
        hand.PlayCard(card, true); // Play essence card
        Debug.Log("Used essence card.");
        return true;
    }

    void ReplaceTimelineEvent(CardDisplay eventToPlay, BoardSpace targetSpace)
    {
        eventToPlay.actionRequest.isBot = true;
        eventToPlay.actionRequest.activeBoardTargets.Add(targetSpace);
        hand.PlayCard(eventToPlay, true); // Play the event card
    }
}
