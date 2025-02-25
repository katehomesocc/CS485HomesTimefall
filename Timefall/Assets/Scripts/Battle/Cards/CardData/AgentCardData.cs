using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Agent CardData", menuName = "CardData/AgentCardData")]
public class AgentCardData : CardData
{
    public string diceType;
    public int diceCost;

    [SerializeField]

    public AgentAction defaultAgentAction;
    public AgentAction agentAction;

    public void Awake()
    {
        cardType = CardType.AGENT;
    }

    public AgentAction GetAgentAction(bool isPlaced)
    {
        if(!isPlaced)
        {
            return defaultAgentAction;
        }

        return agentAction;
    }

}