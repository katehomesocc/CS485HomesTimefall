using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MediumBotAI : BotAI
{
    protected override void AnalyzeBoard()
    {
        Debug.Log("MediumBot is analyzing the board.");

        turnCycleSpaces = boardManager.GetBoardSpacesSortedByFactionVictoryPoints(botPlayer.faction, true);
        allSpaces = boardManager.GetBoardSpacesSortedByFactionVictoryPoints(botPlayer.faction, false);

        Debug.Log($"Turn Cycle [{turnCycleSpaces.Count}] | All [{allSpaces.Count}]");
        Debug.Log("Prioritized spaces analyzed.");

        currentState = BotState.ChooseAction;
    }

    protected override void ChooseAction()
    {
        Debug.Log("MediumBot is choosing an action.");

        foreach (var cardDisplay in hand.displaysInHand)
        {
            // if (cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay, true)) // Turn Cycle
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }

            if (cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay, true)) // Turn Cycle
            {
                currentState = BotState.ExecuteAction;
                return;
            }
        }

        foreach (var cardDisplay in hand.displaysInHand)
        {
            // if (cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay, false)) // Whole Board
            // {
            //     currentState = BotState.ExecuteAction;
            //     return;
            // }

            if (cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay, false)) // Whole Board
            {
                currentState = BotState.ExecuteAction;
                return;
            }
        }

        // if (ChooseEssenceAction())
        // {
        //     currentState = BotState.ExecuteAction;
        //     return;
        // }

        Debug.Log("No valid actions. Ending turn.");
        currentState = BotState.EndTurn;
    }

    protected override IEnumerator ExecuteAction()
    {
        yield return null;
    }

    private bool ChooseEssenceAction()
    {
        if (TryUseEssenceOfType(ActionType.Revive)) return true;
        if (TryUseEssenceOfType(ActionType.Swap)) return true;
        if (TryUseEssenceOfType(ActionType.Shield)) return true;
        if (TryUseEssenceOfType(ActionType.Paradox)) return true;
        if (TryUseEssenceOfType(ActionType.CosmicBlast)) return true;
        if (TryUseEssenceOfType(ActionType.Convert)) return true;
        if (TryUseEssenceOfType(ActionType.Channel)) return true;

        return false;
    }

    private bool TryUseEssenceOfType(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Revive:
                return TryUseRevive();
            case ActionType.Swap:
                if (currentTurnCycle == 1) break;
                return TryUseSwap();
        }
        return false;
    }

    private bool TryUseRevive()
    {
        foreach (CardDisplay cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.Revive) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            
            var discardPile = botPlayer.deck.discardPile; 

            Card agentToRevive = discardPile.FirstOrDefault(card => card.GetCardType() == CardType.AGENT);

            if (agentToRevive == null) return false;

            // Cast cardDisplay to EssenceCardDisplay to access action request
            EssenceCardDisplay essenceCardDisplay = (EssenceCardDisplay) cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            // Set the revive target
            actionRequest.discardedTarget = agentToRevive;

            // Play the revive action
            StartCoroutine(ReviveAgent(cardDisplay, agentToRevive));

            Debug.Log($"Revived agent: {agentToRevive.data.cardName} for faction {botPlayer.faction}");

            return true;
        }

        // No revive card was played
        return false;
    }

    private bool TryUseSwap()
    {
        foreach (var cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.Swap) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            var essenceCardDisplay = (EssenceCardDisplay)cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            // Find the lowest VP card in the turn cycle that is targetable
            var lowestInCycle = turnCycleSpaces
                .Where(space => actionRequest.potentialBoardTargets.Contains(space))
                .OrderBy(space => space.eventCard.eventCardData.victoryPoints[playerNumber])
                .FirstOrDefault();

            if (lowestInCycle == null) return false;

            // Find the highest VP card out of the turn cycle that is targetable
            var highestOutOfCycle = allSpaces
                .Except(turnCycleSpaces)
                .Where(space => actionRequest.potentialBoardTargets.Contains(space))
                .OrderByDescending(space => space.eventCard.eventCardData.victoryPoints[playerNumber])
                .FirstOrDefault();

            if (highestOutOfCycle == null) return false;

            if (lowestInCycle.eventCard.eventCardData.victoryPoints[playerNumber] >=
                highestOutOfCycle.eventCard.eventCardData.victoryPoints[playerNumber])
            {
                return false;
            }

            StartCoroutine(SwapEvents(cardDisplay, lowestInCycle, highestOutOfCycle));
            Debug.Log("Used SWAP essence card.");
            return true;
        }

        return false;
    }

//     private void AnalyzeBoard()
//     {
//         Debug.Log("MediumBot is analyzing the board.");

//         // Get sorted board spaces for Turn Cycle and Whole Board
//         turnCycleSpaces = BoardManager.Instance.GetBoardSpacesSortedByFactionVictoryPoints(botPlayer.faction, true);
//         allSpaces = BoardManager.Instance.GetBoardSpacesSortedByFactionVictoryPoints(botPlayer.faction, false);

//         Debug.Log($"Turn Cycle [{turnCycleSpaces.Count}] | All [{allSpaces.Count}]");

//         Debug.Log("Prioritized spaces analyzed.");
//         currentState = BotState.ChooseAction;
//     }

//     private void ChooseAction()
//     {
//         Debug.Log("MediumBot is choosing an action.");

//         // Evaluate hand for priority actions based on Turn Cycle and Whole Board
//         foreach (var cardDisplay in hand.displaysInHand)
//         {
//             // if (cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay, true)) // Turn Cycle
//             // {
//             //     currentState = BotState.ExecuteAction;
//             //     return;
//             // }

//             // if (cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay, true)) // Turn Cycle
//             // {
//             //     currentState = BotState.ExecuteAction;
//             //     return;
//             // }
//         }

//         foreach (var cardDisplay in hand.displaysInHand)
//         {
//             // if (cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay, false)) // Whole Board
//             // {
//             //     currentState = BotState.ExecuteAction;
//             //     return;
//             // }

//             // if (cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay, false)) // Whole Board
//             // {
//             //     currentState = BotState.ExecuteAction;
//             //     return;
//             // }
//         }

//         if(ChooseEssenceAction())
//         {
//             currentState = BotState.ExecuteAction;
//             return;         
//         }

//         // If no actions are possible, end turn
//         Debug.Log("No valid actions. Ending turn.");
//         currentState = BotState.EndTurn;
//     }

//     private IEnumerator ExecuteAction()
//     {
//         // Debug.Log("MediumBot is executing an action.");
//         yield return null;
//     }

}
