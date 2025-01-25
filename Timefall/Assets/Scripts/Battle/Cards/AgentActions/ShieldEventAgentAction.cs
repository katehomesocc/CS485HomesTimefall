using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New DEFAULT Agent Action", menuName = "Agent Action/EVENT SHIELD")]
[System.Serializable]
public class EventShieldAgentAction : AgentAction
{
    public Texture SHIELD_TEX;

    public Texture2D CURSOR_SHIELD_TEX;

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

        //event must not be sheilded
        if(boardSpace.eventCard.shielded) { return false ;}

        return true;
    }

    public override List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        List<BoardSpace> targetableSpaces = new List<BoardSpace>();

        if(actionRequest.activeBoardTargets.Count == 1){ return targetableSpaces;}

        Debug.Log(string.Format("actionRequest.potentialBoardTargets.Count [{0}]", actionRequest.potentialBoardTargets.Count));

        foreach (BoardSpace boardSpace in actionRequest.potentialBoardTargets)
        {
            Debug.Log(boardSpace.name);

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
            return SHIELD_TEX;
        }

        return null;
    }

    Texture2D GetCursorTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeBoardTargets.Count == 0)
        {
            return CURSOR_SHIELD_TEX;
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
            Shield(activeBoardTargets, actionRequest);
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

        hand.EndAgentAction();
    }

    private void Shield(List<BoardSpace> boardTargets, ActionRequest actionRequest)
    {
        Hand.Instance.SetHandState(HandState.ACTION_START);
        
        //TODO: need to account for if this agent dies, to remove the shield frome the agent
        BoardSpace target = boardTargets[0];

        Player owner = actionRequest.player;

        AgentCard agent = (AgentCard) actionRequest.actionCard;
        
        Shield shield = new Shield(owner, Expiration.NONE, target, true, false);
        
        //if this agent dies, to remove the shield frome the event
        shield.SubscribeToAgent(agent.onDeath);
        target.EventEquiptShield(shield);

        SendChatLogMessage(actionRequest.player, agent.data, target.eventCard.data);

        AudioManager.Instance.Play(audioClip);
        
        EndAction(actionRequest);
    }

    void SendChatLogMessage(Player player, CardData agentData, CardData eventData)
    {
        ChatMessageData data = new ChatMessageData(player, ChatMessageData.Action.AgentShield);

        data.cards.Add(agentData);
        data.cards.Add(eventData);
        data.expiration = Expiration.NONE;

        ChatLogManager.Instance.SendMessage(data);
    }
}
