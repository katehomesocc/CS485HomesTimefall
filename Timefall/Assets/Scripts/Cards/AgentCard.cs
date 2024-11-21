using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCard : Card
{
    public AgentCardData agentCardData;

    public bool isOnBoard = false;

    public AgentCard (AgentCardData cardData)
    {
        this.data = cardData;
        this.agentCardData = cardData;
    }

    private void Awake() {
        agentCardData = (AgentCardData) data;
    }

    public override void SelectBoardTarget(ActionRequest actionRequest)
    {
        isOnBoard = true;
        actionRequest.boardTarget.SetAgentCard(this);
        BattleManager.Instance.ClearPossibleTargetHighlights(actionRequest);
        Hand.Instance.RemoveCardAfterPlaying(false);
    }

    public List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        
        List<BoardSpace> targetableSpaces = new List<BoardSpace>();

        if(actionRequest.potentialBoardTargets == null)
        {
            return targetableSpaces;
        }

        foreach (BoardSpace boardSpace in actionRequest.potentialBoardTargets)
        {
            if(!CanTargetSpace(boardSpace)) { continue;}

            targetableSpaces.Add(boardSpace);
        }

        return targetableSpaces;
    }

    public List<Card> GetTargatableDiscardedCards(ActionRequest actionRequest)
    {
        
        List<Card> argatableDiscardedCards = new List<Card>();

        if(actionRequest.potentialDiscardedTargets == null)
        {
            return argatableDiscardedCards;
        }

        foreach (Card card in actionRequest.potentialDiscardedTargets)
        {
            if(!CanTargetDiscardedCard(card)) { continue;}

            argatableDiscardedCards.Add(card);
        }

        return argatableDiscardedCards;
    }

    bool CanTargetSpace(BoardSpace boardSpace)
    {
        
        //must have an event & not have an agent
        if(!boardSpace.hasEvent || boardSpace.hasAgent) { return false ;}

        return true;
    }

    bool CanTargetDiscardedCard(Card card)
    {
        
        return false;
    }


    public override bool CanBePlayed(ActionRequest potentialTargetsRequest)
    {
        if(isOnBoard)
        {
            return false;
        }

        return potentialTargetsRequest.potentialBoardTargets.Count > 0; 
    }
}
