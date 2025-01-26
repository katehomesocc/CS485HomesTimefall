using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Essence CardData", menuName = "CardData/EssenceCardData")]
public class EssenceCardData : CardData
{
    public ActionType actionType;
    [SerializeField]
    public EssenceAction essenceAction;
    public void Awake()
    {
        cardType = CardType.ESSENCE;
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
