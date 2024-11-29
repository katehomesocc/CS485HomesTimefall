using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AgentRCD : ResearchCardDisplay
{
    [Header("Agent")]
    public TMP_Text factionText;
    public TMP_Text diceTypeText;
    public TMP_Text diceCostText;
    public Image diceImage;
    public Image borderImage;

    void ResetDisplay(AgentCardData cardData) 
    {
        //base card
        nameText.text = cardData.cardName;
        descText.text = cardData.description;

        image.texture = cardData.image;

        
        //dice text
        diceTypeText.text = cardData.diceType;
        diceCostText.text = cardData.diceCost.ToString();
    
        SetFactionText(cardData.faction);
        SetFactionColors(ResearchManager.GetFactionColor(cardData.faction));
    }

    public void SetCard(AgentCardData cardData)
    {
        ResetDisplay(cardData);
    }


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

}
