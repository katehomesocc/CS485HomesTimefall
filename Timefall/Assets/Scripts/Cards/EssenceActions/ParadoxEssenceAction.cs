using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New PARADOX Essence Action", menuName = "Essence Action/PARADOX")]
[System.Serializable]
public class ParadoxEssenceAction : EssenceAction
{
    public Texture PARADOX_TEX;

    public Texture2D CURSOR_PARADOX_TEX;

    public override bool CanBePlayed(ActionRequest actionRequest)
    {
        return actionRequest.potentialBoardTargets.Count >= 1; 
    }

    bool CanTargetSpace(BoardSpace boardSpace)
    {   
        //must have an event & not being shielded 
        if(!boardSpace.hasEvent || boardSpace.shielded) { return false ;}

        return true;
    }

    public override List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        List<BoardSpace> targetableSpaces = new List<BoardSpace>();

        if(actionRequest.activeBoardTargets.Count == 1){ return targetableSpaces;}

        foreach (BoardSpace boardSpace in actionRequest.potentialBoardTargets)
        {
            if(!CanTargetSpace(boardSpace)) { continue;}

            targetableSpaces.Add(boardSpace);
        }

        return targetableSpaces;
    }

    public override List<CardDisplay> GetTargatableHandDisplays(ActionRequest actionRequest)
    {
        return new List<CardDisplay>();
    }
    
    public override List<Card> GetTargatableDiscardedCards(ActionRequest actionRequest)
    {
        return new List<Card>();
    }

    public override Texture GetSelectionTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeBoardTargets.Count == 0)
        {
            return PARADOX_TEX;
        }

        return null;
    }

    Texture2D GetCursorTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeBoardTargets.Count == 0)
        {
            return CURSOR_PARADOX_TEX;
        }

        return null;
    }

    public override void SelectBoardTarget(ActionRequest actionRequest)
    {
        List<BoardSpace> activeBoardTargets = actionRequest.activeBoardTargets;
        if(activeBoardTargets.Count == 0)
        {
            BoardSpace boardTarget =  actionRequest.boardTarget;
            activeBoardTargets.Add(boardTarget);

            Cursor.SetCursor(GetCursorTexture(actionRequest), Vector2.zero, CursorMode.Auto);

            Texture selectionTexture = GetSelectionTexture(actionRequest);
            boardTarget.SelectAsTarget(selectionTexture);
        }
        
        Hand.Instance.UpdatePossibilities(actionRequest);

        if(activeBoardTargets.Count == 1)
        {
            Paradox(activeBoardTargets, actionRequest);
        }
    }

    public override void SelectHandTarget(ActionRequest actionRequest)
    {
        return;
    }

    public override void SelectDiscardedTarget(ActionRequest actionRequest)
    {
        return;
    }


    public override void StartAction(ActionRequest actionRequest)
    {
        Cursor.SetCursor(GetCursorTexture(actionRequest), Vector2.zero, CursorMode.Auto);
    }

    public override void EndAction(ActionRequest actionRequest)
    {
        //set handstate
        Hand hand = Hand.Instance;
        hand.SetHandState(HandState.ACTION_END);
        
        //reset cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        //reset boardTargets
        foreach (var targetedSpace in actionRequest.activeBoardTargets)
        {
            targetedSpace.DeselectAsTarget();
        }

        hand.RemoveCardAfterPlaying();
    }

    private void Paradox(List<BoardSpace> boardTargets, ActionRequest actionRequest)
    {
        BoardSpace target = boardTargets[0];

        //send event to timeline discard
        EventCard eventToDiscard = target.eventCard;
        target.RemoveEventCard();
        BattleManager.Instance.DiscardToDeck(eventToDiscard, Faction.NONE);

        if(target.hasAgent)
        {
            //send agent to its factions discard pile
            AgentCard agentToDiscard = target.agentCard;
            target.RemoveAgentCard();
            BattleManager.Instance.DiscardToDeck(agentToDiscard, agentToDiscard.GetFaction());
        }
        
        EndAction(actionRequest);
    }
}
