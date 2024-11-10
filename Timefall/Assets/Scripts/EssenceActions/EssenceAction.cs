using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EssenceAction : ScriptableObject 
{
    public List<BoardSpace> targets = new List<BoardSpace>();
    public abstract void SelectTarget(BoardSpace boardSpace, List<BoardSpace> targets);
    public abstract Texture GetSelectionTexture(List<BoardSpace> targets); 
    public abstract List<BoardSpace> GetTargatableSpaces(List<BoardSpace> boardSpaces, List<BoardSpace> targets);

    public abstract bool CanTargetSpace(BoardSpace boardSpaces, List<BoardSpace> targets);

    public abstract void StartAction(List<BoardSpace> targets);
    public abstract void EndAction(List<BoardSpace> targets);

}
