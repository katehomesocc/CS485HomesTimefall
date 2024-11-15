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

    public override bool CanBePlayed(List<BoardSpace> potentialBoardTargets, List<CardDisplay> potentialHandTargets)
    {
        return potentialBoardTargets.Count >= 2; 
    }

    public override bool CanTargetSpace(BoardSpace boardSpace, List<BoardSpace> boardTargets)
    {   
        //must have an event & not have an agent & not already targeted
        if(!boardSpace.hasEvent || boardSpace.hasAgent|| boardSpace.isBeingTargeted) { return false ;}

        return true;
    }

    public override bool CanTargetHandDisplay(CardDisplay handDisplay, List<CardDisplay> handTargets)
    { 
        return false;
    }

    /* [SWAP]
     * 1. Targetable Space Criteria:
     *      - Has an Event
     *      - Doesnt have an agent
     * 2. Atleast 2 Targetable Spaces
     *      - if not, return empty list
    */
    public override List<BoardSpace> GetTargatableSpaces(List<BoardSpace> spacesToTest, List<BoardSpace> boardTargets)
    {
        List<BoardSpace> targetableSpaces = new List<BoardSpace>();

        if(boardTargets.Count == 2){ return targetableSpaces;}

        foreach (BoardSpace boardSpace in spacesToTest)
        {
            if(!CanTargetSpace(boardSpace, boardTargets)) { continue;}

            targetableSpaces.Add(boardSpace);
        }

        //Must have atleast 2 targetable spaces on board to swap
        if(targetableSpaces.Count + boardTargets.Count < 2)
        {
            Debug.Log("SwapEA: not enough boardTargets");
            targetableSpaces.Clear();
        }

        return targetableSpaces;
    }

    public override List<CardDisplay> GetTargatableHandDisplays(List<CardDisplay> handDisplays, List<CardDisplay> handTargets)
    {
        return new List<CardDisplay>();
    }

    public override Texture GetSelectionTexture(List<BoardSpace> boardTargets, List<CardDisplay> handTargets)
    {
        if(boardTargets.Count == 0)
        {
            return CARD_TAP_UP_TEX;
        } else if (boardTargets.Count == 1) {
            return CARD_TAP_DOWN_TEX;
        }

        return null;
    }

    public Texture2D GetCursorTexture(List<BoardSpace> boardTargets)
    {
        if(boardTargets.Count == 0)
        {
            return CURSOR_CARD_TAP_UP_TEX;
        } else if (boardTargets.Count == 1) {
            return CURSOR_CARD_TAP_DOWN_TEX;
        }

        return null;
    }

    public override void SelectTarget(BoardSpace boardSpace, List<BoardSpace> boardTargets, List<CardDisplay> handTargets, Player player)
    {
        if(boardTargets.Count < 2)
        {
            boardTargets.Add(boardSpace);

            Cursor.SetCursor(GetCursorTexture(boardTargets), Vector2.zero, CursorMode.Auto);

            Texture selectionTexture = GetSelectionTexture(boardTargets, handTargets);
            boardSpace.SelectAsTarget(selectionTexture);
        }
        
        Hand.Instance.UpdatePossibilities();

        if(boardTargets.Count == 2)
        {
            Swap(boardTargets);
        }
    }

    public override void SelectTarget(CardDisplay handTarget, List<BoardSpace> boardTargets, List<CardDisplay> handTargets, Player player)
    {
        return;
    }

    public override void StartAction(List<BoardSpace> boardTargets, List<CardDisplay> handTargets, Player player)
    {
        Cursor.SetCursor(GetCursorTexture(boardTargets), Vector2.zero, CursorMode.Auto);
    }

    public override void EndAction(List<BoardSpace> boardTargets, List<CardDisplay> handTargets)
    {
        //set handstate
        Hand hand = Hand.Instance;
        hand.SetHandState(HandState.ACTION_END);
        
        //reset cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        //reset boardTargets
        foreach (var targetedSpace in boardTargets)
        {
            targetedSpace.DeselectAsTarget();
        }

        hand.RemoveCardAfterPlaying();
    }

    private void Swap(List<BoardSpace> boardTargets)
    {
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
        EndAction(boardTargets, null);
    }
}
