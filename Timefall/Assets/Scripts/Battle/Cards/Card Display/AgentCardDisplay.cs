using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AgentCardDisplay : CardDisplay
{
    [Header("Agent")]
    public TMP_Text factionText;
    public TMP_Text diceTypeText;
    public TMP_Text diceCostText;
    public Image diceImage;
    public Image borderImage;

    // Start is called before the first frame update
    void Start()
    {
        //ResetDisplay((AgentCard) displayCard);
    }

    void ResetDisplay(AgentCard agentCard) 
    {
        AgentCardData cardData = agentCard.agentCardData;
        //base card
        nameText.text = cardData.cardName;
        descText.text = cardData.description;

        image.texture = cardData.image;

        
        //dice text
        diceTypeText.text = cardData.diceType;
        diceCostText.text = cardData.diceCost.ToString();
    
        SetFactionText(cardData.faction);
        SetFactionColors(BattleManager.GetFactionColor(cardData.faction));
        
    }

    public void SetCard(AgentCard agentCard)
    {
        displayCard = agentCard;
        ResetDisplay(agentCard);
        actionRequest.actionCard = agentCard;
        actionRequest.doBoard = true;
    }

    // public void SetCard(Card agentCard)
    // {
    //     SetCard((AgentCard) agentCard);
    // }

    void SetFactionText(Faction faction)
    {
        switch(faction) 
        {
            case Faction.WEAVERS:
                factionText.text = "the Weaver";
                break;
            case Faction.SEEKERS:
                factionText.text = "the Seeker";
                break;
            case Faction.SOVEREIGNS:
                factionText.text = "the Sovereign";
                break;
            case Faction.STEWARDS:
                factionText.text = "the Steward";
                break;
            default:
                Debug.LogError("Invalid Faction");
                break;
        }
    }

    void SetFactionColors(Color color)
    {
        diceImage.color = color;
        borderImage.color = color;
        factionText.color = color;
    }

    public AgentCard GetAgentCard()
    {
        return (AgentCard) displayCard;
    }

    public AgentCard PlayFromHand()
    {
        Vector3 newPosition = this.transform.position + new Vector3(0, 50, 0);
        StartCoroutine(this.MoveToPosition(newPosition, 0.25f));

        GetAgentCard().StartAction(actionRequest);
        
        return GetAgentCard();
    }

}