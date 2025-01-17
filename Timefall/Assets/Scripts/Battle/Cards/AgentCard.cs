using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCard : Card
{
    public AgentCardData agentCardData;

    public bool isOnBoard = false;
    public bool attempted = false;

    public AgentCard (AgentCardData cardData)
    {
        this.data = cardData;
        this.agentCardData = cardData;
    }

    private void Awake() {
        agentCardData = (AgentCardData) data;
    }

    public AgentAction GetAgentAction()
    {
        return agentCardData.GetAgentAction(isOnBoard);
    }

    public List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        return GetAgentAction().GetTargatableSpaces(actionRequest);
    }

    public List<CardDisplay> GetTargatableHandDisplays(ActionRequest actionRequest)
    {
        return GetAgentAction().GetTargatableHandDisplays(actionRequest);
    }

    public List<Card> GetTargatableDiscardedCards(ActionRequest actionRequest)
    {
        return GetAgentAction().GetTargatableDiscardedCards(actionRequest);
    }

    public override void SelectBoardTarget(ActionRequest actionRequest)
    {
        GetAgentAction().SelectBoardTarget(actionRequest);
    }

    public override void SelectHandTarget(ActionRequest actionRequest)
    {
        GetAgentAction().SelectHandTarget(actionRequest);
    }

    public override void SelectDiscardTarget(ActionRequest actionRequest)
    {
        GetAgentAction().SelectDiscardedTarget(actionRequest);
    }

    public void SetActionRequest(ActionRequest actionRequest)
    {
        actionRequest.actionCard = this;
        if(GetAgentAction() == null) 
        {
            Debug.Log("AgentCard.SetActionRequest essence action === null");
        }
        GetAgentAction().SetActionRequest(actionRequest);
    }

    public void StartAction(ActionRequest actionRequest)
    {
        GetAgentAction().StartAction(actionRequest);
    }

    
    public override bool CanBePlayed(ActionRequest potentialTargetsRequest)
    {
        return GetAgentAction().CanBePlayed(potentialTargetsRequest);
    }

    public void RollForAction()
    {
        int cost = agentCardData.diceCost;
        DoDiceRoll(cost);
    }

    public void SuccessCallback()
    {

        Debug.Log("AgentCard callback ... success!");
    
    }
    public void FailureCallback()
    {
        Debug.Log("AgentCard callback ... failure");
    }

    void DoDiceRoll(int cost)
    {
        switch (agentCardData.diceType)
        {
            case "D4":
                BattleManager.Instance.diceRoller.RollD4(cost, this);
                break;
            case "D6":
                BattleManager.Instance.diceRoller.RollD6(cost, this);
                break;
            case "D8":
                BattleManager.Instance.diceRoller.RollD8(cost, this);
                break;
            default:
                return;
        }
    }

}
