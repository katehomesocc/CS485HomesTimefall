using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceCard : Card
{
    // public new EssenceCardData data;
    public EssenceCardData essenceCardData;

    public EssenceCard (EssenceCardData cardData)
    {
        this.data = cardData;
        this.essenceCardData = cardData;
    }

    void Awake() {
        essenceCardData = (EssenceCardData) data;
    }

    EssenceAction GetEssenceAction()
    {
        return essenceCardData.essenceAction;
    }

    public List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        //TODO: add functionality
        return essenceCardData.GetTargatableSpaces(actionRequest);
    }

    public List<CardDisplay> GetTargatableHandDisplays(ActionRequest actionRequest)
    {
        //TODO: add functionality
        return essenceCardData.GetTargatableHandDisplays(actionRequest);
    }

    public List<Card> GetTargatableDiscardedCards(ActionRequest actionRequest)
    {
        //TODO: add functionality
        return essenceCardData.GetTargatableDiscardedCards(actionRequest);
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

    public void StartAction(ActionRequest actionRequest)
    {
        GetEssenceAction().StartAction(actionRequest);
    }

    
    public override bool CanBePlayed(ActionRequest potentialTargetsRequest)
    {
        return GetEssenceAction().CanBePlayed(potentialTargetsRequest);
    }

}
