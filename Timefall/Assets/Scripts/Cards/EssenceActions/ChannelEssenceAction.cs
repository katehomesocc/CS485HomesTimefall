using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New CHANNEL Essence Action", menuName = "Essence Action/CHANNEL")]
[System.Serializable]
public class ChannelEssenceAction : EssenceAction
{
    public Texture CHANNEL_TEX;
    public Texture2D CURSOR_CHANNEL_TEX;

    public override bool CanBePlayed(List<BoardSpace> potentialBoardTargets, List<CardDisplay> potentialHandTargets)
    {
        Debug.Log(string.Format("potentialHandTargets.Count: [{0}]", potentialHandTargets.Count));
        return potentialHandTargets.Count >= 1; 
    }

    public override bool CanTargetSpace(BoardSpace boardSpace, List<BoardSpace> boardTargets)
    {   
        return false;
    }

    public override bool CanTargetHandDisplay(CardDisplay handDisplay, List<CardDisplay> handTargets)
    { 
        //can target card if not already channeling  
        if(!handDisplay.displayCard.channeling)
        {
            return true;
        }

        return false;
    }

    public override List<BoardSpace> GetTargatableSpaces(List<BoardSpace> spacesToTest, List<BoardSpace> boardTargets)
    {
        return new List<BoardSpace>();
    }

    public override List<CardDisplay> GetTargatableHandDisplays(List<CardDisplay> handDisplaysToTest, List<CardDisplay> handTargets)
    {
        List<CardDisplay> targetableDisplays = new List<CardDisplay>();

        foreach (CardDisplay display in handDisplaysToTest)
        {
            if(!CanTargetHandDisplay(display, handTargets)) { continue;}

            targetableDisplays.Add(display);
        }

        return targetableDisplays;
    }

    public override Texture GetSelectionTexture(List<BoardSpace> boardTargets, List<CardDisplay> handTargets)
    {
        if(handTargets.Count == 0)
        {
            return CHANNEL_TEX;
        } 
        return null;
    }

    public Texture2D GetCursorTexture(List<BoardSpace> boardTargets, List<CardDisplay> handTargets)
    {
        if(handTargets.Count == 0)
        {
            return CURSOR_CHANNEL_TEX;
        } 
        return null;
    }

    public override void SelectTarget(BoardSpace boardSpace, List<BoardSpace> boardTargets, List<CardDisplay> handTargets, Player player)
    {
        return;
    }

    public override void SelectTarget(CardDisplay handTarget, List<BoardSpace> boardTargets, List<CardDisplay> handTargets, Player player)
    {
        if(handTargets.Count != 0)
        {
            return;
        } 

        handTargets.Add(handTarget);

        Cursor.SetCursor(GetCursorTexture(null, handTargets), Vector2.zero, CursorMode.Auto);

        Channel(handTarget, handTargets, player);

        return;
    }



    public override void StartAction(List<BoardSpace> boardTargets, List<CardDisplay> handTargets, Player player)
    {
        Cursor.SetCursor(GetCursorTexture(null, handTargets), Vector2.zero, CursorMode.Auto);
    }

    public override void EndAction(List<BoardSpace> boardTargets, List<CardDisplay> handTargets)
    {
        //set handstate
        Hand hand = Hand.Instance;
        hand.SetHandState(HandState.ACTION_END);
        
        //reset cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        //reset targets
        foreach (CardDisplay targetCard in handTargets)
        {
            targetCard.DeselectAsTarget();
        }

        hand.RemoveCardAfterPlaying();
    }

    private void Channel(CardDisplay handTarget, List<CardDisplay> handTargets, Player player)
    {

        player.ChannelCard(handTarget);

        //end of action
        EndAction(null, handTargets);
    }
}
