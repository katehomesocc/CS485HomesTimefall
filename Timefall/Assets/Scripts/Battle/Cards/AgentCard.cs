using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AgentCard : Card
{
    public UnityEvent onDeath = new UnityEvent();
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

    public void RollForAction(AgentIcon agentIcon)
    {
        int cost = agentCardData.diceCost;
        DoDiceRoll(cost, agentIcon);
    }

    public void SuccessCallback(ActionRequest actionRequest)
    {
        Debug.Log("AgentCard callback ... success!");

        StartAction(actionRequest);
    }
    
    public void FailureCallback()
    {
        Debug.Log("AgentCard callback ... failure");
    }

    void DoDiceRoll(int cost, AgentIcon agentIcon)
    {
        switch (agentCardData.diceType)
        {
            case "D4":
                BattleManager.Instance.diceRoller.RollD4(cost, agentIcon);
                break;
            case "D6":
                BattleManager.Instance.diceRoller.RollD6(cost, agentIcon);
                break;
            case "D8":
                BattleManager.Instance.diceRoller.RollD8(cost, agentIcon);
                break;
            default:
                return;
        }
    }

    public void Death()
    {
        isOnBoard = false;
        onDeath.Invoke();
    }

    public override void EquipShield(Shield equip)
    {
        shield = equip;
        shielded = true;

        equip.SubscribeToAgent(onDeath);
    }

}
