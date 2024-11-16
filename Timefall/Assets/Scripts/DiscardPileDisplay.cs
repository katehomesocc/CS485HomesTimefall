using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPileDisplay : MonoBehaviour
{
    public GameObject inventoryGameObject;
    public GameObject inventoryPanel;
    public List<CardDisplay> displaysShowing = new List<CardDisplay>();
    
    BattleManager battleManager;

    GameObject agentCardDisplay;
    GameObject essenceCardDisplay;
    GameObject eventCardDisplay;

    public Player player;



    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        agentCardDisplay = battleManager.agentCardDisplay;
        essenceCardDisplay = battleManager.essenceCardDisplay;
        eventCardDisplay = battleManager.eventCardDisplay;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInventory(List<Card> cardsToDisplay)
    {
        foreach (Card card in cardsToDisplay)
        {
            InstantiateCard(card);
        }
    }

    public void ClearInventory()
    {
        while (displaysShowing.Count > 0) {
            CardDisplay cardDisplay = displaysShowing[0];
            displaysShowing.RemoveAt(0);
            DestroyImmediate(cardDisplay.transform.gameObject);
        }
    }

    public void OpenInventory()
    {
        inventoryGameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        inventoryGameObject.SetActive(false);
    }

    CardDisplay InstantiateCard(Card card)
    {
        CardType cardType = card.data.cardType;

        CardDisplay returnDisplay;
        
        switch(cardType) 
        {
            case CardType.AGENT:
                returnDisplay = InstantiateAgentInInventory((AgentCard) card);
                break;
            case CardType.ESSENCE:
                returnDisplay = InstantiateEssenceInInventory((EssenceCard) card);
                break;
            case CardType.EVENT:
                returnDisplay = InstantiateEventInInventory((EventCard) card);

                break;
            default:
            //Error handling
                Debug.LogError("Invalid CardData Type: " + cardType);
                return null;
        }

        // displaysShowing.Add(card);
        return returnDisplay;
    }

    CardDisplay InstantiateAgentInInventory(AgentCard agentCard)
    {
        GameObject obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

        if(agentDC == null){return null;} 

        agentDC.SetCard(agentCard);
        agentDC.InstantiateInInventory(inventoryPanel.transform);
        agentDC.positionInHand = displaysShowing.Count;

        displaysShowing.Add(agentDC);
        
        return agentDC;
    }

    CardDisplay InstantiateEssenceInInventory(EssenceCard essenceCard)
    {
        GameObject obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

        if(essenceDC == null){return null;} 

        essenceDC.SetCard(essenceCard);
        essenceDC.InstantiateInInventory(inventoryPanel.transform);
        essenceDC.positionInHand = displaysShowing.Count;
        displaysShowing.Add(essenceDC);

        return essenceDC;
    }

    CardDisplay InstantiateEventInInventory(EventCard eventCard)
    {
        GameObject obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

        if(eventDC == null){return null;} 

        eventDC.SetCard(eventCard);
        eventDC.InstantiateInInventory(inventoryPanel.transform);
        eventDC.positionInHand = displaysShowing.Count;
        displaysShowing.Add(eventDC);

        return eventDC;
    }

    public List<Card> GetPossibleTargets(Card card, ActionRequest actionRequest)
    {
        switch(card.GetCardType()) 
        {
            case CardType.AGENT:
                // return GetAgentPossibilities((AgentCard) card);
                break;
            case CardType.ESSENCE:
                return GetEssencePossibilities((EssenceCard) card, actionRequest);
            case CardType.EVENT:
                break;
            default:
            //Error handling
                Debug.LogError("Invalid Card Type: " + card.data.cardType);
                break;
        }
        return new List<Card>();
    }

    public List<Card> GetEssencePossibilities(EssenceCard essenceCard, ActionRequest actionRequest)
    {   
        actionRequest.potentialDiscardedTargets = player.deck.discardPile;
        return essenceCard.GetTargatableDiscardedCards(actionRequest);
    }
}
