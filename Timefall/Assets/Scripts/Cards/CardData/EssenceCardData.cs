using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Essence CardData", menuName = "CardData/EssenceCardData")]
public class EssenceCardData : CardData
{
    [SerializeField]
    public EssenceAction essenceAction;
    public void Awake()
    {
        cardType = CardType.ESSENCE;
    }

    public bool CanTargetSpace(BoardSpace boardSpaces, List<BoardSpace> boardTargets)
    {
        //TODO: add functionality
        return essenceAction.CanTargetSpace(boardSpaces, boardTargets);
    }

    public List<BoardSpace> GetTargatableSpaces(List<BoardSpace> board, List<BoardSpace> boardTargets)
    {
        //TODO: add functionality
        return essenceAction.GetTargatableSpaces(board, boardTargets);
    }

    public List<CardDisplay> GetTargatableHandDisplays(List<CardDisplay> handDisplays, List<CardDisplay> handTargets)
    {
        //TODO: add functionality
        return essenceAction.GetTargatableHandDisplays(handDisplays, handTargets);
    }

}
