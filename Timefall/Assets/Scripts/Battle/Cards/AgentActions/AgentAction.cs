using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentAction : ScriptableObject {

    public AudioClip audioClip;
    public abstract bool CanBePlayed(ActionRequest ActionRequest);
    public abstract Texture GetSelectionTexture(ActionRequest ActionRequest);

    public abstract void SelectBoardTarget(ActionRequest ActionRequest);
    public abstract void SelectHandTarget(ActionRequest ActionRequest);
    public abstract void SelectDiscardedTarget(ActionRequest ActionRequest);
    
    public abstract List<BoardSpace> GetTargatableSpaces(ActionRequest ActionRequest);
    public abstract List<CardDisplay> GetTargatableHandDisplays(ActionRequest ActionRequest);
    public abstract List<Card> GetTargatableDiscardedCards(ActionRequest ActionRequest);

    public abstract void SetActionRequest(ActionRequest actionRequest);
    public abstract void StartAction(ActionRequest ActionRequest);
    public abstract void EndAction(ActionRequest ActionRequest);

}
