using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New DEFAULT Agent Action", menuName = "Agent Action/PATCH")]
[System.Serializable]
public class PatchAgentAction : AgentAction
{
    public Texture PATCH_TEX;

    public Texture2D CURSOR_PATCH_TEX;

    public override bool CanBePlayed(ActionRequest actionRequest)
    {
        return actionRequest.potentialBoardTargets.Count >= 1; 
    }

    bool CanTargetSpace(BoardSpace boardSpace)
    {   
        if(!boardSpace.isUnlocked)
        {
            return false;
        }

        if(!boardSpace.isHole) { return false ;}

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
            return PATCH_TEX;
        }

        return null;
    }

    Texture2D GetCursorTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeBoardTargets.Count == 0)
        {
            return CURSOR_PATCH_TEX;
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
            Patch(activeBoardTargets, actionRequest);
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
        BattleManager.Instance.SetPossibleTargetHighlights(actionRequest.actionCard, actionRequest);
        Cursor.SetCursor(GetCursorTexture(actionRequest), Vector2.zero, CursorMode.Auto);
    }

    public override void EndAction(ActionRequest actionRequest)
    {
        Hand hand = Hand.Instance;
        //set handstate
        hand.SetHandState(HandState.ACTION_END);
        
        //reset cursor
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        //reset boardTargets
        foreach (var targetedSpace in actionRequest.activeBoardTargets)
        {
            targetedSpace.DeselectAsTarget();
        }

        hand.EndAgentAction();
    }

    private void Patch(List<BoardSpace> boardTargets, ActionRequest actionRequest)
    {
        Hand.Instance.SetHandState(HandState.ACTION_START);
    
        BoardSpace target = boardTargets[0];

        //Draw card from timeline deck
        EventCardDisplay display = Hand.Instance.PatchDisplayFromTimelineDeck();

        SendChatLogMessage(actionRequest.player, actionRequest.actionCard.data, display.displayCard.data);

        target.Patch(display);
        
        AudioManager.Instance.Play(audioClip);
        
        EndAction(actionRequest);
    }

    void SendChatLogMessage(Player player, CardData agent, CardData eventTarget)
    {
        ChatMessageData data = new ChatMessageData(player, ChatMessageData.Action.PatchEvent);

        data.cards.Add(agent);
        data.cards.Add(eventTarget);

        ChatLogManager.Instance.SendMessage(data);
    }
}
