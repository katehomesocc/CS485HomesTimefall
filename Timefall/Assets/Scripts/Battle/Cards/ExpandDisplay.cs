using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandDisplay : MonoBehaviour
{
    [Header("Expanded Card View")]
    public Transform expandedHoverTransform;

    public Transform expandedStaticTransform;

    public List<CardDisplay> staticCards = new List<CardDisplay>();

    BattleManager battleManager;

    GameObject agentCardDisplay;
    GameObject essenceCardDisplay;
    GameObject eventCardDisplay;

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
        agentCardDisplay = battleManager.agentCardDisplay;
        essenceCardDisplay = battleManager.essenceCardDisplay;
        eventCardDisplay = battleManager.eventCardDisplay;
    }

    public CardDisplay ExpandCardView(Card card, bool hoverClear)
    {
        if(card == null){ return null;}

        CardType cardType = card.data.cardType;

        CardDisplay displayToReturn = null;

        Transform expandedViewTransform = hoverClear ? expandedHoverTransform :  expandedStaticTransform;

        switch(cardType) 
        {
            case CardType.AGENT:

                displayToReturn = InstantiateAgentExpand((AgentCard) card, expandedViewTransform);

                break;
            case CardType.ESSENCE:
                displayToReturn = InstantiateEssenceExpand((EssenceCard) card, expandedViewTransform);

                break;
            case CardType.EVENT:
                displayToReturn = InstantiateEventInExpand((EventCard) card, expandedViewTransform);

                break;
            default:
            //Error handling
                Debug.LogError("Invalid CardData Type: " + cardType);
                return null;
        }

        if(!hoverClear)
        {
            staticCards.Add(displayToReturn);
        } else
        {
            CanvasGroup canvasGroup = displayToReturn.gameObject.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
        }

        //Debug.Break();

        return displayToReturn;
    }

    public void CloseExpandCardView()
    {
        while (expandedHoverTransform.childCount > 0) {
            
            DestroyImmediate(expandedHoverTransform.GetChild(0).gameObject);
        }
    }

    CardDisplay InstantiateAgentExpand(AgentCard agentCard, Transform expandParent)
    {
        GameObject obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

        if(agentDC == null){return null;} 

        agentDC.SetCard(agentCard);
        agentDC.InstantiateInExpand(expandParent);
        
        return agentDC;
    }

    CardDisplay InstantiateEssenceExpand(EssenceCard essenceCard, Transform expandParent)
    {
        GameObject obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

        if(essenceDC == null){return null;} 

        essenceDC.SetCard(essenceCard);
        essenceDC.InstantiateInExpand(expandParent);

        return essenceDC;
    }

    CardDisplay InstantiateEventInExpand(EventCard eventCard, Transform expandParent)
    {
        GameObject obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
        EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

        if(eventDC == null){return null;} 

        eventDC.SetCard(eventCard);
        eventDC.InstantiateInExpand(expandParent);

        return eventDC;
    }

    public void PlayInitialTimelineCard(CardDisplay display)
    {
        if(staticCards.Count == 1)
        {
            Hand.Instance.PlayInitialTimelineCard(display);
            staticCards.RemoveAt(0);
        }
    }

    public void AutoPlayInitialTimelineCard()
    {
        if(staticCards.Count == 1)
        {
            Hand.Instance.PlayInitialTimelineCard(staticCards[0]);
            staticCards.RemoveAt(0);
        }
    }

}
