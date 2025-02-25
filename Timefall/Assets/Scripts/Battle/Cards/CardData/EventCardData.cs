using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event CardData", menuName = "CardData/EventCardData")]
public class EventCardData : CardData
{
    [SerializeField]
    public EssenceAction essenceAction;
    public int[] victoryPoints = new int[4]; //0: Stewards, 1: Seekers, 2: Sovereigns, 3: Weavers

    public void Awake()
    {
        cardType = CardType.EVENT;
    }

    public List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        return essenceAction.GetTargatableSpaces(actionRequest);
    }

    public List<CardDisplay> GetTargatableHandDisplays(ActionRequest actionRequest)
    {
        return essenceAction.GetTargatableHandDisplays(actionRequest);
    }

    public List<Card> GetTargatableDiscardedCards(ActionRequest actionRequest)
    {
        return essenceAction.GetTargatableDiscardedCards(actionRequest);
    }

    
}
