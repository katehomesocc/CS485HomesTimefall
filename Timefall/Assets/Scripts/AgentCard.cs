using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Agent Card", menuName = "AgentCard")]
public class AgentCard : Card
{
    public string diceType;
    public int diceCost;

    public void Awake()
    {
        cardType = CardType.AGENT;
    }

}