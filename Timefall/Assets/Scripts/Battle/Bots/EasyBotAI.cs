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

        // Try to play an agent card second
        // foreach (CardDisplay cardDisplay in shuffledHand)
        // {
        //     if (cardDisplay.GetCardType() == CardType.AGENT && TryPlayAgentCard(cardDisplay))
        //     {
        //         currentState = BotState.ExecuteAction;
        //         return;
        //     }
        // }

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

    private bool TryPlayEssenceCard(CardDisplay card)
    {
        if (!card.CanBePlayed(botPlayer)) return false;
        
        hand.PlayCard(card, true); // Play essence card immediately
        Debug.Log($"Easy Bot used essence card [{card.displayCard.data.cardName}].");
        return true;
    }
}
