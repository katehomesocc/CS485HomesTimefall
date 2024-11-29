using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchScrollableDisplay : MonoBehaviour
{
    ResearchManager researchManager;
    public Transform contentPanel;

    GameObject agentCardDisplay;
    GameObject essenceCardDisplay;
    GameObject eventCardDisplay;

    // Start is called before the first frame update
    void Start()
    {
        researchManager = ResearchManager.Instance;
        agentCardDisplay = researchManager.agentCardDisplay;
        essenceCardDisplay = researchManager.essenceCardDisplay;
        eventCardDisplay = researchManager.eventCardDisplay;
    }

    public void SetContent(List<CardData> cardDataList)
    {
        foreach (CardData data in cardDataList)
        {
            InstantiateCard(data);
        }
    }

    ResearchCardDisplay InstantiateCard(CardData cardData)
    {
        CardType cardType = cardData.cardType;

        ResearchCardDisplay returnDisplay;
        
        switch(cardType) 
        {
            case CardType.AGENT:
                returnDisplay = InstantiateAgentInContent((AgentCardData) cardData);
                break;
            case CardType.ESSENCE:
                returnDisplay = InstantiateEssenceInContent((EssenceCardData) cardData);
                break;
            case CardType.EVENT:
                returnDisplay = InstantiateEventInContent((EventCardData) cardData);

                break;
            default:
            //Error handling
                Debug.LogError("Invalid CardData Type: " + cardType);
                return null;
        }

        return returnDisplay;
    }

    AgentRCD InstantiateAgentInContent(AgentCardData agentCard)
    {
        GameObject obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        AgentRCD agentRDC = obj.GetComponent<AgentRCD>();

        if(agentRDC == null){return null;} 

        agentRDC.SetCard(agentCard);
        agentRDC.AddToContentPanel(contentPanel);
        
        return agentRDC;
    }

    EssenceRCD InstantiateEssenceInContent(EssenceCardData essenceCard)
    {
        GameObject obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        EssenceRCD essenceRDC = obj.GetComponent<EssenceRCD>();

        if(essenceRDC == null){return null;} 

        essenceRDC.SetCard(essenceCard);
        essenceRDC.AddToContentPanel(contentPanel);

        return essenceRDC;
    }

    EventRCD InstantiateEventInContent(EventCardData eventCard)
    {
        GameObject obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        EventRCD eventRDC = obj.GetComponent<EventRCD>();

        if(eventRDC == null){return null;} 

        eventRDC.SetCard(eventCard);
        eventRDC.AddToContentPanel(contentPanel);

        return eventRDC;
    }
}
