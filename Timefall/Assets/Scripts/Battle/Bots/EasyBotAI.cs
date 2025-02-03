using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class EasyBotAI : BotAI
{
    protected override void AnalyzeBoard()
    {
        Debug.Log("Easy Bot is analyzing the board.");

        // Get board spaces but do not prioritize points
        allSpaces = boardManager.GetUnlockedBoardSpaces();

        // Debug.Log($"Turn Cycle [{turnCycleSpaces.Count}] | All [{allSpaces.Count}]");

        currentState = BotState.ChooseAction;
    }

    protected override void ChooseAction()
    {
        Debug.Log("Easy Bot is choosing an action.");

        var random = new System.Random();
        List<CardDisplay> shuffledHand = hand.displaysInHand.OrderBy(x => random.Next()).ToList();

        // Try to play an event card first
        foreach (CardDisplay cardDisplay in shuffledHand)
        {
            if (cardDisplay.GetCardType() == CardType.EVENT && TryPlayEventCard(cardDisplay))
            {
                currentState = BotState.ExecuteAction;
                return;
            }
        }

        //Try to play an agent card second
        foreach (CardDisplay cardDisplay in shuffledHand)
        {
            if (cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay))
            {
                currentState = BotState.ExecuteAction;
                return;
            }
        }

        // Try to play an essence card last
        foreach (CardDisplay cardDisplay in shuffledHand)
        {
            if (cardDisplay.GetCardType() == CardType.ESSENCE && TryPlayEssenceCard(cardDisplay))
            {
                currentState = BotState.ExecuteAction;
                return;
            }
        }

        Debug.Log("Easy Bot has no valid actions. Ending turn.");
        currentState = BotState.EndTurn;
    }

    protected override IEnumerator ExecuteAction()
    {
        yield return null;
    }

    private bool TryPlayEssenceCard(CardDisplay cardDisplay)
    {
        switch (cardDisplay.GetActionType())
        {
            case ActionType.Channel:
                return TryUseChannel(cardDisplay);
            case ActionType.Convert:
                return TryUseConvert(cardDisplay);
            case ActionType.CosmicBlast:
                return TryUseCosmicBlast(cardDisplay);
            case ActionType.Paradox:
                return TryUseParadox(cardDisplay);
            case ActionType.Revive:
                return TryUseRevive(cardDisplay);
            case ActionType.Shield:
                return TryUseShield(cardDisplay);
            case ActionType.Swap:
                if (currentTurnCycle == 1) break;
                return TryUseSwap(cardDisplay);
        }
        return false;
    }

    private bool TryPlayEventCard(CardDisplay card)
    {
        // Get a random unlocked board space
        List<BoardSpace> validSpaces = allSpaces.Where(space => (space.hasEvent)).ToList();
        if (validSpaces.Count == 0) return false;

        BoardSpace targetSpace = validSpaces[Random.Range(0, validSpaces.Count)];
        StartCoroutine(ReplaceTimelineEvent(card, targetSpace));

        Debug.Log($"Easy Bot played event card [{card.displayCard.data.cardName}] on space #{targetSpace.spaceNumber}.");
        return true;
    }

    private bool TryPlayAgentCard(CardDisplay card)
    {
        List<BoardSpace> validSpaces = allSpaces.Where(space => space.hasEvent && !space.hasAgent).ToList();
        if (validSpaces.Count == 0) return false;

        var targetSpace = validSpaces[Random.Range(0, validSpaces.Count)];
        StartCoroutine(PlaceAgent(card, targetSpace));

        Debug.Log($"Easy Bot deployed agent [{card.displayCard.data.cardName}] on space #{targetSpace.spaceNumber}.");
        return true;
    }

    private bool TryUseChannel(CardDisplay cardDisplay)
    {
        if (!cardDisplay.CanBePlayed(botPlayer)) return false;

        List<CardDisplay> potentialHandTargets = cardDisplay.actionRequest.potentialHandTargets;

        CardDisplay cardToChannel = potentialHandTargets
            .OrderBy(_ => Random.value) 
            .FirstOrDefault();

        if (cardToChannel == null) return false;

        StartCoroutine(Channel(cardDisplay, cardToChannel));

        return true;
    }

    private bool TryUseConvert(CardDisplay cardDisplay)
    {
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

    private bool TryUseCosmicBlast(CardDisplay cardDisplay)
    {
        if (!cardDisplay.CanBePlayed(botPlayer)) return false;

        BoardSpace targetSpace = allSpaces
            .Where(space => space.hasEvent && space.hasAgent && space.agentCard.GetFaction() != botPlayer.faction)
            .OrderBy(_ => Random.value)
            .FirstOrDefault();

        if (targetSpace == null) return false;

        StartCoroutine(CosmicBlast(cardDisplay, targetSpace));

        return true;
    }

    private bool TryUseParadox(CardDisplay cardDisplay)
    {
        if (!cardDisplay.CanBePlayed(botPlayer)) return false;

        var essenceCardDisplay = (EssenceCardDisplay)cardDisplay;
        ActionRequest actionRequest = essenceCardDisplay.actionRequest;

        var targetSpace = allSpaces
            .Where(space => space.hasEvent && space.eventCard.GetFaction() != faction && !space.shielded)
            .OrderBy(_ => Random.value)
            .FirstOrDefault();

        if (targetSpace == null) return false;
        
        StartCoroutine(Paradox(cardDisplay, targetSpace));

        Debug.Log($"Paradox used on {targetSpace.eventCard.eventCardData.cardName}, creating a hole in time.");

        return true;
    }

    private bool TryUseShield(CardDisplay cardDisplay)
    {
        if (!cardDisplay.CanBePlayed(botPlayer)) return false;

        var targetSpaces = allSpaces
            .Where(space => space.hasEvent && space.hasAgent && space.agentCard.GetFaction() == botPlayer.faction)
            .OrderBy(_ => Random.value)
            .ToList();

        if (targetSpaces.Count == 0) return false;

        BoardSpace targetSpace = targetSpaces.First();
        StartCoroutine(Shield(cardDisplay, targetSpace));

        return true;
    }

    private bool TryUseSwap(CardDisplay cardDisplay)
    {
        if (!cardDisplay.CanBePlayed(botPlayer)) return false;

        EssenceCardDisplay essenceCardDisplay = (EssenceCardDisplay)cardDisplay;
        ActionRequest actionRequest = essenceCardDisplay.actionRequest;

        List<BoardSpace> targetableSpaces = actionRequest.potentialBoardTargets;

        if (targetableSpaces.Count < 2) return false;

        // Select two random spaces for swapping
        BoardSpace space1 = targetableSpaces.OrderBy(_ => Random.value).First();

        BoardSpace space2 = targetableSpaces.Where(space => space != space1).OrderBy(_ => Random.value).FirstOrDefault();

        if (space2 == null) return false;

        StartCoroutine(SwapEvents(cardDisplay, space1, space2));
        return true;

    }



}
