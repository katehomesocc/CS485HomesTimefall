using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AgentCardDisplay : CardDisplay
{
    public static Color COLOUR_SEEKERS = new Color(33f/255,197f/255,104f/255, 1f);
    public static Color COLOUR_SOVEREIGNS = new Color(255f/255,35f/255,147f/255, 1f);
    public static Color COLOUR_STEWARDS = new Color(24f/255,147f/255,248f/255, 1f);
    public static Color COLOUR_WEAVERS = new Color(97f/255,65f/255,172f/255, 1f);

    public TMP_Text factionText;
    public TMP_Text diceTypeText;
    public TMP_Text diceCostText;

    public List<Card> agentDB;
    public int pos = 0;

    public Image diceImage;
    public Image borderImage;

    // Start is called before the first frame update
    void Start()
    {
        agentDB = GameObject.FindGameObjectWithTag("AgentDB").GetComponent<CardDatabase>().cardList;
        displayCard = agentDB[0];
        ResetDisplay((AgentCard) displayCard);
    }

    void ResetDisplay(AgentCard agentCard) 
    {
        //base card
        nameText.text = agentCard.cardName;
        descText.text = agentCard.description;

        image.texture = agentCard.image;

        
        //dice text
        diceTypeText.text = agentCard.diceType;
        diceCostText.text = agentCard.diceCost.ToString();
    
        SetFactionText(agentCard.faction);
        SetFactionColors(GetFactionColor(agentCard.faction));
        
    }

    void SetCard(AgentCard agentCard)
    {
        displayCard = agentCard;
        ResetDisplay(agentCard);
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
                Debug.Log("Invalid Faction");
                break;
        }
    }

    Color GetFactionColor(Faction faction)
    {
        switch(faction) 
        {
            case Faction.WEAVERS:
                return COLOUR_WEAVERS;
            case Faction.SEEKERS:
                return COLOUR_SEEKERS;
            case Faction.SOVEREIGNS:
                return COLOUR_SOVEREIGNS;
            case Faction.STEWARDS:
                return COLOUR_STEWARDS;
        }

        return Color.black;
    }

    void SetFactionColors(Color color)
    {
        diceImage.color = color;
        borderImage.color = color;
        factionText.color = color;
    }

    public void ShowNextCard()
    {
        if(pos < agentDB.Count-1)
        {
            pos++;
        } 
        else
        {
            pos = 0;
        }

        SetCard((AgentCard) agentDB[pos]);

    }

}
