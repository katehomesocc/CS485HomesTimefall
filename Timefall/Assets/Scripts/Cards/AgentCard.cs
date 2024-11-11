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

    public override void SelectTarget(BoardSpace boardSpace)
    {
        isOnBoard = true;
        boardSpace.SetAgentCard(this);
        BoardManager.Instance.ClearPossibilities();
        Hand.Instance.RemoveCardAfterPlaying();
    }

    public List<BoardSpace> GetTargatableSpaces(List<BoardSpace> spacesToTest)
    {
        List<BoardSpace> targetableSpaces = new List<BoardSpace>();

        foreach (BoardSpace boardSpace in spacesToTest)
        {
            if(!CanTargetSpace(boardSpace)) { continue;}

            targetableSpaces.Add(boardSpace);
        }

        return targetableSpaces;
    }

    public bool CanTargetSpace(BoardSpace boardSpace)
    {
        
        //must have an event & not have an agent
        if(!boardSpace.hasEvent || boardSpace.hasAgent) { return false ;}

        return true;
    }

    public override bool CanBePlayed()
    {
        if(isOnBoard)
        {
            return false;
        }

        int potentialTargets = BoardManager.Instance.GetAgentPossibilities(this).Count;

        return potentialTargets > 0; 
    }
}
