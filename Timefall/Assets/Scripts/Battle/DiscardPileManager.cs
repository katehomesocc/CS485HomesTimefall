using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPileManager : MonoBehaviour
{
    public static DiscardPileManager Instance;
    public DiscardPileDisplay stewardDisplay;
    public DiscardPileDisplay seekerDisplay;
    public DiscardPileDisplay sovereignDisplay;
    public DiscardPileDisplay weaverDisplay;

    List<DiscardPileDisplay> discardDisplays;
    
    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }

        discardDisplays = new List<DiscardPileDisplay>{stewardDisplay, seekerDisplay, sovereignDisplay, weaverDisplay};
    }

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
        Debug.LogError("how did you get here???");
        return new List<Card>();
    }

    public List<Card> GetEssencePossibilities(EssenceCard essenceCard, ActionRequest request)
    {   
        Debug.Log(string.Format("getting essence possibilities for {0}", essenceCard.GetCardName()));
        List<Card> possibleTargets = new List<Card>();

        possibleTargets.AddRange(stewardDisplay.GetPossibleTargets(essenceCard, request));
        possibleTargets.AddRange(seekerDisplay.GetPossibleTargets(essenceCard, request));
        possibleTargets.AddRange(sovereignDisplay.GetPossibleTargets(essenceCard, request));
        possibleTargets.AddRange(weaverDisplay.GetPossibleTargets(essenceCard, request));
        return possibleTargets;
    }

    public void SetPossibleTargets(Card card, ActionRequest actionRequest)
    {
        stewardDisplay.SetPossibleTargets(card, actionRequest);
        seekerDisplay.SetPossibleTargets(card, actionRequest);
        sovereignDisplay.SetPossibleTargets(card, actionRequest);
        weaverDisplay.SetPossibleTargets(card, actionRequest);

    }

    public void ClearAndClosePossibleTargets()
    {
        stewardDisplay.ClearAndCloseInventory();
        seekerDisplay.ClearAndCloseInventory();
        sovereignDisplay.ClearAndCloseInventory();
        weaverDisplay.ClearAndCloseInventory();
    }

    public CardDisplay GetCardDisplayForDiscardedCard(Card targetCard)
    {
        foreach (DiscardPileDisplay discardDisplay in discardDisplays)
        {
            CardDisplay cardDisplay = discardDisplay.GetCardDisplayForDiscardedCard(targetCard);
            if(discardDisplay != null) return cardDisplay;
        }

        return null; 
    }
}
