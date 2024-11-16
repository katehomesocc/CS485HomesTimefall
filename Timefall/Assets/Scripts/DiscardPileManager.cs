using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPileManager : MonoBehaviour
{
    public DiscardPileDisplay stewardDisplay;
    public DiscardPileDisplay seekerDisplay;
    public DiscardPileDisplay sovereignDisplay;
    public DiscardPileDisplay weaverDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDisplays()
    {
        stewardDisplay.transform.gameObject.SetActive(true);
        seekerDisplay.transform.gameObject.SetActive(true);
        sovereignDisplay.transform.gameObject.SetActive(true);
        weaverDisplay.transform.gameObject.SetActive(true);
    }
    public void HideDisplays()
    {
        stewardDisplay.transform.gameObject.SetActive(false);
        seekerDisplay.transform.gameObject.SetActive(false);
        sovereignDisplay.transform.gameObject.SetActive(false);
        weaverDisplay.transform.gameObject.SetActive(false);
    }

    public void ShowFactionPile(Faction faction)
    {
        GameObject pileObj = GetFactionPileDisplay(faction).transform.gameObject;

        pileObj.SetActive(true);
    }

    public DiscardPileDisplay GetFactionPileDisplay(Faction faction)
    {
        switch (faction)
        {
            case Faction.STEWARDS:
                return stewardDisplay;
            case Faction.SEEKERS:
                return seekerDisplay;
            case Faction.SOVEREIGNS:
                return sovereignDisplay;   
            case Faction.WEAVERS:
                return weaverDisplay;    
        }

        return null;
    }

        public List<Card> GetPossibleTargets(Card card, ActionRequest request)
    {
        switch(card.GetCardType()) 
        {
            case CardType.AGENT:
                // return GetAgentPossibilities((AgentCard) card);
                break;
            case CardType.ESSENCE:
                return GetEssencePossibilities((EssenceCard) card, request);
            case CardType.EVENT:
                break;
            default:
            //Error handling
                Debug.LogError("Invalid Card Type: " + card.data.cardType);
                break;
        }
        return new List<Card>();
    }

    public List<Card> GetEssencePossibilities(EssenceCard essenceCard, ActionRequest request)
    {   
        List<Card> possibleTargets = new List<Card>();

        possibleTargets.AddRange(stewardDisplay.GetPossibleTargets(essenceCard, request));
        possibleTargets.AddRange(seekerDisplay.GetPossibleTargets(essenceCard, request));
        possibleTargets.AddRange(sovereignDisplay.GetPossibleTargets(essenceCard, request));
        possibleTargets.AddRange(weaverDisplay.GetPossibleTargets(essenceCard, request));
        return possibleTargets;
    }
}
