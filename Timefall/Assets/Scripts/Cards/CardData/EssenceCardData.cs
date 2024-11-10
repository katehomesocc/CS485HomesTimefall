using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Essence CardData", menuName = "EssenceCardData")]
public class EssenceCardData : CardData
{
    [SerializeField]
    public EssenceAction essenceAction;
    public void Awake()
    {
        cardType = CardType.ESSENCE;
    }

    public bool CanTargetSpace(BoardSpace boardSpaces, List<BoardSpace> targets)
    {
        //TODO: add functionality
        return essenceAction.CanTargetSpace(boardSpaces, targets);
    }

    public List<BoardSpace> GetTargatableSpaces(List<BoardSpace> board, List<BoardSpace> targets)
    {
        //TODO: add functionality
        return essenceAction.GetTargatableSpaces(board, targets);
    }

}
