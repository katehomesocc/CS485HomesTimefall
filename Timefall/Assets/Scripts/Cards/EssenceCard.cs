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

    public bool CanTargetSpace(BoardSpace boardSpaces)
    {
        //TODO: add functionality
        return essenceCardData.CanTargetSpace(boardSpaces, boardTargets);
    }

    public List<BoardSpace> GetTargatableSpaces(List<BoardSpace> board)
    {
        //TODO: add functionality
        return essenceCardData.GetTargatableSpaces(board, boardTargets);
    }

    public List<CardDisplay> GetTargatableHandDisplays(List<CardDisplay> handDisplays, List<CardDisplay> handTargets)
    {
        //TODO: add functionality
        return essenceCardData.GetTargatableHandDisplays(handDisplays, handTargets);
    }

    public override void SelectTarget(BoardSpace boardSpace)
    {
        GetEssenceAction().SelectTarget(boardSpace, boardTargets, handTargets);
    }

    public override void SelectTarget(CardDisplay handDisplay)
    {
        GetEssenceAction().SelectTarget(handDisplay, boardTargets, handTargets);
    }

    public void StartAction()
    {
        GetEssenceAction().StartAction(boardTargets, handTargets);
    }

    
    public override bool CanBePlayed()
    {
        List<BoardSpace> potentialBoardTargets = BoardManager.Instance.GetPossibleTargets(this);
        List<CardDisplay> potentialHandTargets = Hand.Instance.GetPossibleTargets(this);
        return GetEssenceAction().CanBePlayed(potentialBoardTargets, potentialHandTargets);
    }

}
