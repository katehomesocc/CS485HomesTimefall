using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Swap Essence Action", menuName = "Swap Essence Action")]
[System.Serializable]
public class SwapEssenceAction : EssenceAction
{
    public Texture CARD_TAP_UP_TEX;
    public Texture CARD_TAP_DOWN_TEX;

    public Texture2D CURSOR_CARD_TAP_UP_TEX;
    public Texture2D CURSOR_CARD_TAP_DOWN_TEX;

    public override bool CanTargetSpace(BoardSpace boardSpace, List<BoardSpace> targets)
    {   
        //must have an event & not have an agent & not already targeted
        if(!boardSpace.hasEvent || boardSpace.hasAgent|| boardSpace.isBeingTargeted) { return false ;}

        return true;
    }

    /* [SWAP]
     * 1. Targetable Space Criteria:
     *      - Has an Event
     *      - Doesnt have an agent
     * 2. Atleast 2 Targetable Spaces
     *      - if not, return empty list
    */
    public override List<BoardSpace> GetTargatableSpaces(List<BoardSpace> spacesToTest, List<BoardSpace> targets)
    {
        List<BoardSpace> targetableSpaces = new List<BoardSpace>();

        if(targets.Count == 2){ return targetableSpaces;}

        foreach (BoardSpace boardSpace in spacesToTest)
        {
            if(!CanTargetSpace(boardSpace, targets)) { continue;}

            targetableSpaces.Add(boardSpace);
        }

        //Must have atleast 2 targetable spaces on board to swap
        if(targetableSpaces.Count + targets.Count < 2)
        {
            Debug.Log("SwapEA: not enough targets");
            targetableSpaces.Clear();
        }

        return targetableSpaces;
    }

    public override Texture GetSelectionTexture(List<BoardSpace> targets)
    {
        if(targets.Count == 0)
        {
            return CARD_TAP_UP_TEX;
        } else if (targets.Count == 1) {
            return CARD_TAP_DOWN_TEX;
        }

        return null;
    }

    public Texture2D GetCursorTexture(List<BoardSpace> targets)
    {
        if(targets.Count == 0)
        {
            return CURSOR_CARD_TAP_UP_TEX;
        } else if (targets.Count == 1) {
            return CURSOR_CARD_TAP_DOWN_TEX;
        }

        return null;
    }

    public override void SelectTarget(BoardSpace boardSpace, List<BoardSpace> targets)
    {
        if(targets.Count < 2)
        {
            targets.Add(boardSpace);

            Cursor.SetCursor(GetCursorTexture(targets), Vector2.zero, CursorMode.Auto);

            Texture selectionTexture = GetSelectionTexture(targets);
            boardSpace.SelectAsTarget(selectionTexture);
        }
        
        Hand.Instance.UpdatePossibilities();

        if(targets.Count == 2)
        {
            Swap(targets);
        }
    }

    public override void StartAction(List<BoardSpace> targets)
    {
        Cursor.SetCursor(GetCursorTexture(targets), Vector2.zero, CursorMode.Auto);
    }

    public override void EndAction(List<BoardSpace> targets)
    {
        //set handstate
        Hand hand = Hand.Instance;
        hand.SetHandState(HandState.ACTION_END);
        
        //reset cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        //reset targets
        foreach (var targetedSpace in targets)
        {
            targetedSpace.DeselectAsTarget();
        }

        hand.RemoveCardAfterPlaying();
    }

    private void Swap(List<BoardSpace> targets)
    {
        //Get both target eventDisplays
        EventCardDisplay target1 = targets[0].eventDisplay;
        EventCardDisplay target2 = targets[1].eventDisplay;

        //get parent and position info for both targets
        Transform newParentT1 = target2.transform.parent;
        Vector3 newLocalPositionT1 = target2.transform.localPosition;

        Transform newParentT2 = target1.transform.parent;
        Vector3 newLocalPositionT2 = target1.transform.localPosition;

        //set parent and position for both targets
        target1.transform.SetParent(newParentT1, false);
        target1.transform.localPosition = newLocalPositionT1;
        // target1.transform.localEulerAngles  = Vector3.zero;

        target2.transform.SetParent(newParentT2, false);
        target2.transform.localPosition = newLocalPositionT2;
        // target1.transform.localEulerAngles  = Vector3.zero;

        targets[1].SetEventCard(target1);
        targets[0].SetEventCard(target2);

        //end of action
        EndAction(targets);
    }
}
