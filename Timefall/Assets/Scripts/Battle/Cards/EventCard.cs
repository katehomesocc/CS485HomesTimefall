using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCard : Card
{
    public EventCardData eventCardData;

    public EventCard (EventCardData cardData)
    {
        this.data = cardData;
        this.eventCardData = cardData;
    }

    private void Awake() {
        eventCardData = (EventCardData) data;
    }


    EssenceAction GetEssenceAction()
    {
        return eventCardData.essenceAction;
    }

    public List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        return eventCardData.GetTargatableSpaces(actionRequest);
    }

    public List<CardDisplay> GetTargatableHandDisplays(ActionRequest actionRequest)
    {
        return eventCardData.GetTargatableHandDisplays(actionRequest);
    }

    public List<Card> GetTargatableDiscardedCards(ActionRequest actionRequest)
    {
        return eventCardData.GetTargatableDiscardedCards(actionRequest);
    }

    public override void SelectBoardTarget(ActionRequest actionRequest)
    {
        GetEssenceAction().SelectBoardTarget(actionRequest);
    }

    public override void SelectHandTarget(ActionRequest actionRequest)
    {
        GetEssenceAction().SelectHandTarget(actionRequest);
    }

        public override void SelectDiscardTarget(ActionRequest actionRequest)
    {
        GetEssenceAction().SelectDiscardedTarget(actionRequest);
    }

    public void SetActionRequest(ActionRequest actionRequest)
    {
        actionRequest.actionCard = this;
        if(GetEssenceAction() == null) 
        {
            Debug.Log("EventCard.SAR essence action === null");
        }
        GetEssenceAction().SetActionRequest(actionRequest);
    }

    public void StartAction(ActionRequest actionRequest)
    {
        GetEssenceAction().StartAction(actionRequest);
    }

    
    public override bool CanBePlayed(ActionRequest potentialTargetsRequest)
    {
        return GetEssenceAction().CanBePlayed(potentialTargetsRequest);
    }
}
