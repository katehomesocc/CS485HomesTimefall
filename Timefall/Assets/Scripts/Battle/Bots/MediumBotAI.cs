using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MediumBotAI : BotAI
{
    protected override void AnalyzeBoard()
    {
        Debug.Log("MediumBot is analyzing the board.");

        turnCycleSpaces = boardManager.GetBoardSpacesSortedByFactionVictoryPoints(faction, true);
        allSpaces = boardManager.GetBoardSpacesSortedByFactionVictoryPoints(faction, false);

        Debug.Log($"Turn Cycle [{turnCycleSpaces.Count}] | All [{allSpaces.Count}]");
        Debug.Log("Prioritized spaces analyzed.");

        agentEnoughEssence = BattleStateMachine.Instance.HasEssenceToPlayCardType(CardType.AGENT);
        eventEnoughEssence = BattleStateMachine.Instance.HasEssenceToPlayCardType(CardType.EVENT);
        essenceEnoughEssence = BattleStateMachine.Instance.HasEssenceToPlayCardType(CardType.ESSENCE);


        currentState = BotState.ChooseAction;
    }

    protected override void ChooseAction()
    {
        Debug.Log("MediumBot is choosing an action.");

        // foreach (CardDisplay cardDisplay in hand.displaysInHand)
        // {
        //     // if (eventEnoughEssence && cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay, true)) // Turn Cycle
        //     // {
        //     //     currentState = BotState.ExecuteAction;
        //     //     return;
        //     // }

        //     if (agentEnoughEssence && cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay, true)) // Turn Cycle
        //     {
        //         currentState = BotState.ExecuteAction;
        //         return;
        //     }
        // }

        // foreach (CardDisplay cardDisplay in hand.displaysInHand)
        // {
        //     // if (eventEnoughEssence && cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay, false)) // Whole Board
        //     // {
        //     //     currentState = BotState.ExecuteAction;
        //     //     return;
        //     // }

        //     if (agentEnoughEssence && cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay, false)) // Whole Board
        //     {
        //         currentState = BotState.ExecuteAction;
        //         return;
        //     }
        // }

        if (essenceEnoughEssence && ChooseEssenceAction())
        {
            currentState = BotState.ExecuteAction;
            return;
        }

        Debug.Log("No valid actions. Ending turn.");
        currentState = BotState.EndTurn;
    }

    protected override IEnumerator ExecuteAction()
    {
        yield return null;
    }

    private bool ChooseEssenceAction()
    {
        // if (TryUseEssenceOfType(ActionType.Revive)) return true;
        // if (TryUseEssenceOfType(ActionType.Swap)) return true;
        // if (TryUseEssenceOfType(ActionType.Shield)) return true;
        // if (TryUseEssenceOfType(ActionType.CosmicBlast)) return true;
        // if (TryUseEssenceOfType(ActionType.Paradox)) return true;
        // if (TryUseEssenceOfType(ActionType.Convert)) return true;
        if (TryUseEssenceOfType(ActionType.Channel)) return true;

        return false;
    }

    private bool TryUseEssenceOfType(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Channel:
                return TryUseChannel();
            case ActionType.Convert:
                return TryUseConvert();
            case ActionType.CosmicBlast:
                return TryUseCosmicBlast();
            case ActionType.Paradox:
                return TryUseParadox();
            case ActionType.Revive:
                return TryUseRevive();
            case ActionType.Shield:
                return TryUseShield();
            case ActionType.Swap:
                if (currentTurnCycle == 1) break;
                return TryUseSwap();
        }
        return false;
    }
    private bool TryUseChannel()
    {
        foreach (CardDisplay cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.Channel) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            List<CardDisplay> potentialHandTargets = cardDisplay.actionRequest.potentialHandTargets;

            // Prioritize which card to channel
            CardDisplay cardToChannel = potentialHandTargets
            .Where(card => card.GetActionType() == ActionType.Paradox ||
                         card.GetActionType() == ActionType.Shield ||
                         card.GetActionType() == ActionType.Revive)
            .OrderByDescending(card => GetChannelPriority(card)) // Sort by priority
            .FirstOrDefault();

            if (cardToChannel == null) return false;

            // Cast cardDisplay to EssenceCardDisplay to access action request
            EssenceCardDisplay essenceCardDisplay = (EssenceCardDisplay) cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            // Play the revive action
            StartCoroutine(Channel(cardDisplay, cardToChannel));

            return true;
        }

        return false;
    }

        
    private bool TryUseConvert()
    {
        foreach (CardDisplay cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.Convert) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            List<Card> discardedAgents = cardDisplay.actionRequest.potentialDiscardedTargets;

            Card agentToConvert = discardedAgents.FirstOrDefault(card => card.GetCardType() == CardType.AGENT);

            if (agentToConvert == null) return false;

            // Cast cardDisplay to EssenceCardDisplay to access action request
            EssenceCardDisplay essenceCardDisplay = (EssenceCardDisplay) cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            // Set the revive target
            actionRequest.discardedTarget = agentToConvert;

            // Play the revive action
            StartCoroutine(ConvertAgent(cardDisplay, agentToConvert));

            return true;
        }

        return false;
    }

    private bool TryUseCosmicBlast()
    {
        foreach (var cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.CosmicBlast) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            var essenceCardDisplay = (EssenceCardDisplay)cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            // Find a board space with an event that has an enemy agent
            var targetSpace = allSpaces
                .Where(space => space.hasEvent && space.hasAgent && space.agentCard.GetFaction() != faction)
                .OrderByDescending(space => BoardManager.GetHighestEnemyVP(space, faction))
                .FirstOrDefault();

            if (targetSpace == null) return false;

            Debug.Log($"Bot = [{faction}], agent =[{targetSpace.agentCard.GetFaction()}]");

            StartCoroutine(CosmicBlast(cardDisplay, targetSpace));

            Debug.Log($"Cosmic Blast used on {targetSpace.eventCard.eventCardData.cardName}, removing enemy agent {targetSpace.agentCard.agentCardData.cardName}");

            return true;
        }

        return false;
    }
    
    private bool TryUseParadox()
    {
        foreach (var cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.Paradox) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            var essenceCardDisplay = (EssenceCardDisplay)cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            var targetSpace = allSpaces
                .Where(space => space.hasEvent && space.eventCard.GetFaction() != faction && !space.shielded)
                .OrderByDescending(space => BoardManager.GetHighestEnemyVP(space, faction))
                .FirstOrDefault();

            if (targetSpace == null) return false;
            
            StartCoroutine(Paradox(cardDisplay, targetSpace));

            Debug.Log($"Paradox used on {targetSpace.eventCard.eventCardData.cardName}, creating a hole in time.");

            return true;
        }

        return false;
    }

    private bool TryUseRevive()
    {
        foreach (CardDisplay cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.Revive) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            
            List<Card> discardPile = botPlayer.deck.discardPile; 

            Card agentToRevive = discardPile.FirstOrDefault(card => card.GetCardType() == CardType.AGENT);

            if (agentToRevive == null) return false;

            // Cast cardDisplay to EssenceCardDisplay to access action request
            EssenceCardDisplay essenceCardDisplay = (EssenceCardDisplay) cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            // Set the revive target
            actionRequest.discardedTarget = agentToRevive;

            // Play the revive action
            StartCoroutine(ReviveAgent(cardDisplay, agentToRevive));

            Debug.Log($"Revived agent: {agentToRevive.data.cardName} for faction {faction}");

            return true;
        }

        // No revive card was played
        return false;
    }

    private bool TryUseShield()
    {
        foreach (var cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.Shield) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            var essenceCardDisplay = (EssenceCardDisplay)cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            var targetSpace = allSpaces
                .Where(space => space.hasEvent && space.hasAgent && space.agentCard.GetFaction() == faction)
                .OrderByDescending(space => space.eventCard.eventCardData.victoryPoints[playerNumber])
                .FirstOrDefault();

            if (targetSpace == null) return false;

            StartCoroutine(Shield(cardDisplay, targetSpace));

            Debug.Log($"Shield used on agent {targetSpace.agentCard.agentCardData.cardName}");

            return true;
        }

        return false;
    }

    private bool TryUseSwap()
    {
        foreach (CardDisplay cardDisplay in hand.displaysInHand)
        {
            if (cardDisplay.GetActionType() != ActionType.Swap) continue;
            if (!cardDisplay.CanBePlayed(botPlayer)) return false;

            EssenceCardDisplay essenceCardDisplay = (EssenceCardDisplay)cardDisplay;
            ActionRequest actionRequest = essenceCardDisplay.actionRequest;

            // Find the lowest VP card in the turn cycle that is targetable
            BoardSpace lowestInCycle = turnCycleSpaces
                .Where(space => actionRequest.potentialBoardTargets.Contains(space))
                .OrderBy(space => space.eventCard.eventCardData.victoryPoints[playerNumber])
                .FirstOrDefault();

            if (lowestInCycle == null) return false;

            // Find the highest VP card out of the turn cycle that is targetable
            BoardSpace highestOutOfCycle = allSpaces
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

    private int GetChannelPriority(CardDisplay card)
    {
        switch (card.GetActionType())
        {
            case ActionType.Paradox:
                return 3; // Highest priority (Disrupts enemy board)
            case ActionType.Shield:
                return 2; // Medium priority (Protects key pieces)
            case ActionType.Revive:
                return 1; // Lowest priority (Restores lost units)
            default:
                return 0;
        }
    }


}
