using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New REPLACE Essence Action", menuName = "Essence Action/REPLACE")]
[System.Serializable]
public class ReplaceEssenceAction : EssenceAction
{
    public Texture REPLACE_TEX;

    public Texture2D CURSOR_REPLACE_TEX;

    public override bool CanBePlayed(ActionRequest actionRequest)
    {

        if(actionRequest.potentialBoardTargets.Count >= 1)
        {
            return true;
        }

        if(actionRequest.isBot)
        {
            return (actionRequest.activeBoardTargets.Count == 1);
        }

        return false; 
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
            return REPLACE_TEX;
        }

        return null;
    }

    Texture2D GetCursorTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeBoardTargets.Count == 0)
        {
            return CURSOR_REPLACE_TEX;
        }

        return null;
    }

    public override void SelectBoardTarget(ActionRequest actionRequest)
    {
        Debug.Log("SelectBoardTarget");
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
            Replace(activeBoardTargets, actionRequest.activeHandTargets[0], actionRequest);
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

    public override void SetActionRequest(ActionRequest actionRequest)
    {
        actionRequest.doBoard = true;
    }
    public override void StartAction(ActionRequest actionRequest)
    {
        Debug.Log("StartAction");
        BattleManager.Instance.SetPossibleTargetHighlights(actionRequest.actionCard, actionRequest);
        
        if(actionRequest.isBot){
            //TODO BOT AI
            SelectBoardTarget(actionRequest);
        } else {
            Cursor.SetCursor(GetCursorTexture(actionRequest), Vector2.zero, CursorMode.Auto);
        }
    }

    public override void EndAction(ActionRequest actionRequest)
    {
        Debug.Log("EndAction");
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

        hand.RemoveCardAfterPlaying(true, false);

        if(actionRequest.isBot)
        {
            actionRequest.player.EndBotAction();
        }
    }

    private void Replace(List<BoardSpace> boardTargets, CardDisplay handTarget, ActionRequest actionRequest)
    {
        
        Hand.Instance.SetHandState(HandState.ACTION_START);
        
        BoardSpace target = boardTargets[0];

        SendChatLogMessage(actionRequest.player, target.eventCard.data, handTarget.displayCard.data);

        AudioManager.Instance.Play(audioClip);

        target.Replace((EventCardDisplay) handTarget, this, actionRequest);
    }

    void SendChatLogMessage(Player player, CardData oldEvent, CardData newEvent)
    {
        ChatMessageData data = new ChatMessageData(player, ChatMessageData.Action.ReplaceEvent);

        data.cards.Add(oldEvent);
        data.cards.Add(newEvent);

        ChatLogManager.Instance.SendMessage(data);
    }
}
