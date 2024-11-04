using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Essence Card", menuName = "EssenceCard")]
public class EssenceCard : Card
{
    [SerializeField]
    public EssenceAction essenceAction;
    public void Awake()
    {
        cardType = CardType.ESSENCE;
    }

    public bool CanTargetSpace(BoardSpace boardSpaces)
    {
        //TODO: add functionality
        return essenceAction.CanTargetSpace(boardSpaces);
    }

    public List<BoardSpace> GetTargatableSpaces(List<BoardSpace> board)
    {
        //TODO: add functionality
        return essenceAction.GetTargatableSpaces(board);
    }

}
