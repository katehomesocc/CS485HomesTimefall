using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EssenceAction : ScriptableObject {
        public abstract bool CanBePlayed(List<BoardSpace> potentialBoardTargets, List<CardDisplay> potentialHandTargets);
    public abstract void SelectTarget(BoardSpace boardSpace, List<BoardSpace> boardTargets, List<CardDisplay> handTargets);
    public abstract void SelectTarget(CardDisplay handTarget, List<BoardSpace> boardTargets, List<CardDisplay> handTargets);
    public abstract Texture GetSelectionTexture(List<BoardSpace> boardTargets, List<CardDisplay> handTargets);
    public abstract List<BoardSpace> GetTargatableSpaces(List<BoardSpace> boardSpaces, List<BoardSpace> boardTargets);

    public abstract List<CardDisplay> GetTargatableHandDisplays(List<CardDisplay> handDisplays, List<CardDisplay> handTargets);

    public abstract bool CanTargetSpace(BoardSpace boardSpaces, List<BoardSpace> boardTargets);
    public abstract bool CanTargetHandDisplay(CardDisplay handDisplay, List<CardDisplay> handTargets);

    public abstract void StartAction(List<BoardSpace> boardTargets, List<CardDisplay> handTargets);
    public abstract void EndAction(List<BoardSpace> boardTargets, List<CardDisplay> handTargets);

}
