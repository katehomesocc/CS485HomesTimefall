using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Agent CardData", menuName = "CardData/AgentCardData")]
public class AgentCardData : CardData
{
    public string diceType;
    public int diceCost;

    public void Awake()
    {
        cardType = CardType.AGENT;
    }

}