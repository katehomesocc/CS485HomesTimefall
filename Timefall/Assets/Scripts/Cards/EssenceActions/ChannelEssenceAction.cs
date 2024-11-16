using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New CHANNEL Essence Action", menuName = "Essence Action/CHANNEL")]
[System.Serializable]
public class ChannelEssenceAction : EssenceAction
{
    public Texture CHANNEL_TEX;
    public Texture2D CURSOR_CHANNEL_TEX;

    public override bool CanBePlayed(ActionRequest actionRequest)
    {
        return actionRequest.potentialHandTargets.Count >= 1; 
    }

    bool CanTargetHandDisplay(CardDisplay handDisplay)
    { 
        //can target card if not already channeling  
        if(!handDisplay.displayCard.channeling)
        {
            return true;
        }

        return false;
    }
    public override List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        return new List<BoardSpace>();
    }

    public override List<CardDisplay> GetTargatableHandDisplays(ActionRequest actionRequest)
    {
        List<CardDisplay> targetableDisplays = new List<CardDisplay>();

        foreach (CardDisplay display in actionRequest.potentialHandTargets)
        {
            if(!CanTargetHandDisplay(display)) { continue;}

            targetableDisplays.Add(display);
        }

        return targetableDisplays;
    }

    public override List<Card> GetTargatableDiscardedCards(ActionRequest actionRequest)
    {
        return new List<Card>();
    }

    public override Texture GetSelectionTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeHandTargets.Count == 0)
        {
            return CHANNEL_TEX;
        } 
        return null;
    }

    Texture2D GetCursorTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeHandTargets.Count == 0)
        {
            return CURSOR_CHANNEL_TEX;
        } 
        return null;
    }

    public override void SelectBoardTarget(ActionRequest actionRequest)
    {
        return;
    }

    public override void SelectHandTarget(ActionRequest actionRequest)
    {
        List<CardDisplay> activeHandTargets = actionRequest.activeHandTargets;

        if(activeHandTargets.Count != 0)
        {
            return;
        } 

        CardDisplay handTarget = actionRequest.handTarget;

        activeHandTargets.Add(handTarget);

        Cursor.SetCursor(GetCursorTexture(actionRequest), Vector2.zero, CursorMode.Auto);

        Channel(handTarget, activeHandTargets, actionRequest.player, actionRequest);

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

        //reset targets
        foreach (CardDisplay targetCard in actionRequest.activeHandTargets)
        {
            targetCard.DeselectAsTarget();
        }

        hand.RemoveCardAfterPlaying();
    }

    private void Channel(CardDisplay handTarget, List<CardDisplay> handTargets, Player player, ActionRequest actionRequest)
    {

        player.ChannelCard(handTarget);

        //end of action
        EndAction(actionRequest);
    }
}
