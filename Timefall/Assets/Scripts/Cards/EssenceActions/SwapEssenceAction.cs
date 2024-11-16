using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New SWAP Essence Action", menuName = "Essence Action/SWAP")]
[System.Serializable]
public class SwapEssenceAction : EssenceAction
{
    public Texture CARD_TAP_UP_TEX;
    public Texture CARD_TAP_DOWN_TEX;

    public Texture2D CURSOR_CARD_TAP_UP_TEX;
    public Texture2D CURSOR_CARD_TAP_DOWN_TEX;

    public override bool CanBePlayed(ActionRequest actionRequest)
    {
        Debug.Log(string.Format("CanBePlayed: {0}", actionRequest.ToString()));
        return actionRequest.potentialBoardTargets.Count >= 2; 
    }

   bool CanTargetSpace(BoardSpace boardSpace)
    {   
        Debug.Log(string.Format("CanTargetSpace hasEvent = {0} hasAgent = {1} isBeingTargeted = {2}",
        boardSpace.hasEvent, boardSpace.hasAgent, boardSpace.isBeingTargeted));
        //must have an event & not have an agent & not already targeted
        if(!boardSpace.hasEvent || boardSpace.hasAgent|| boardSpace.isBeingTargeted) { return false ;}

        Debug.Log("canTargetSpace!");
        return true;
    }

    /* [SWAP]
     * 1. Targetable Space Criteria:
     *      - Has an Event
     *      - Doesnt have an agent
     * 2. Atleast 2 Targetable Spaces
     *      - if not, return empty list
    */
    public override List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        Debug.Log("SwapEA GPS: " + actionRequest.ToString());
        List<BoardSpace> targetableSpaces = new List<BoardSpace>();

        List<BoardSpace> activeBoardTargets = actionRequest.activeBoardTargets;

        if(activeBoardTargets.Count == 2){ return targetableSpaces;}

        foreach (BoardSpace boardSpace in actionRequest.potentialBoardTargets)
        {
            if(!CanTargetSpace(boardSpace)) { continue;}

            targetableSpaces.Add(boardSpace);
        }

        //Must have atleast 2 targetable spaces on board to swap
        if(targetableSpaces.Count + activeBoardTargets.Count < 2)
        {
            Debug.Log("SwapEA GPS: not enough boardTargets");
            targetableSpaces.Clear();
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
            return CARD_TAP_UP_TEX;
        } else if (actionRequest.activeBoardTargets.Count == 1) {
            return CARD_TAP_DOWN_TEX;
        }

        return null;
    }

    Texture2D GetCursorTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeBoardTargets.Count == 0)
        {
            return CURSOR_CARD_TAP_UP_TEX;
        } else if (actionRequest.activeBoardTargets.Count == 1) {
            return CURSOR_CARD_TAP_DOWN_TEX;
        }

        return null;
    }
    public override void SelectBoardTarget(ActionRequest actionRequest)
    {
        List<BoardSpace> activeBoardTargets = actionRequest.activeBoardTargets;
        if(activeBoardTargets.Count < 2)
        {
            BoardSpace boardTarget =  actionRequest.boardTarget;
            activeBoardTargets.Add(boardTarget);

            Cursor.SetCursor(GetCursorTexture(actionRequest), Vector2.zero, CursorMode.Auto);

            Texture selectionTexture = GetSelectionTexture(actionRequest);
            boardTarget.SelectAsTarget(selectionTexture);
        }
        Debug.Log("-----------");
        
        Hand.Instance.UpdatePossibilities(actionRequest);

        if(activeBoardTargets.Count == 2)
        {
            Swap(actionRequest);
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

    private void Swap(ActionRequest actionRequest)
    {
        List<BoardSpace> boardTargets = actionRequest.activeBoardTargets;

        //Get both target eventDisplays
        EventCardDisplay target1 = boardTargets[0].eventDisplay;
        EventCardDisplay target2 = boardTargets[1].eventDisplay;

        //get parent and position info for both boardTargets
        Transform newParentT1 = target2.transform.parent;
        Vector3 newLocalPositionT1 = target2.transform.localPosition;

        Transform newParentT2 = target1.transform.parent;
        Vector3 newLocalPositionT2 = target1.transform.localPosition;

        //set parent and position for both boardTargets
        target1.transform.SetParent(newParentT1, false);
        target1.transform.localPosition = newLocalPositionT1;
        // target1.transform.localEulerAngles  = Vector3.zero;

        target2.transform.SetParent(newParentT2, false);
        target2.transform.localPosition = newLocalPositionT2;
        // target1.transform.localEulerAngles  = Vector3.zero;

        boardTargets[1].SetEventCard(target1);
        boardTargets[0].SetEventCard(target2);

        //end of action
        EndAction(actionRequest);
    }
}
