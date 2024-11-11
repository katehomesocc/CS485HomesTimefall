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
        return essenceCardData.CanTargetSpace(boardSpaces, targets);
    }

    public List<BoardSpace> GetTargatableSpaces(List<BoardSpace> board)
    {
        //TODO: add functionality
        return essenceCardData.GetTargatableSpaces(board, targets);
    }

    public override void SelectTarget(BoardSpace boardSpace)
    {
        GetEssenceAction().SelectTarget(boardSpace, targets);
    }

    public void StartAction()
    {
        GetEssenceAction().StartAction(targets);
    }

    
    public override bool CanBePlayed()
    {

        int potentialTargets = BoardManager.Instance.GetEssencePossibilities(this).Count;

        return potentialTargets > 0; 
    }

}
