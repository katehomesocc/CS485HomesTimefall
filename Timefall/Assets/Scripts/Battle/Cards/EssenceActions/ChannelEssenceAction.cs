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

        if(!actionRequest.isBot) { Cursor.SetCursor(GetCursorTexture(actionRequest), Vector2.zero, CursorMode.Auto); }

        Channel(handTarget, activeHandTargets, actionRequest.player, actionRequest);

        return;
    }

    public override void SelectDiscardedTarget(ActionRequest actionRequest)
    {
        return;
    }


    public override void SetActionRequest(ActionRequest actionRequest)
    {
        actionRequest.doHand = true;
    }
    
    public override void StartAction(ActionRequest actionRequest)
    {
        actionRequest.doHand = true;
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

        //reset targets
        foreach (CardDisplay targetCard in actionRequest.activeHandTargets)
        {
            targetCard.DeselectAsTarget();
        }

        hand.RemoveCardAfterPlaying(true,true);
        
        if(actionRequest.isBot)
        {
            actionRequest.player.EndBotAction();
        }
    }

    private void Channel(CardDisplay handTarget, List<CardDisplay> handTargets, Player player, ActionRequest actionRequest)
    {
        Hand.Instance.SetHandState(HandState.ACTION_START);
        
        player.ChannelCard(handTarget);
        
        SendChatLogMessage(player);

        AudioManager.Instance.Play(audioClip);

        //end of action
        EndAction(actionRequest);
    }

    void SendChatLogMessage(Player player)
    {
        ChatMessageData data = new ChatMessageData(player, ChatMessageData.Action.Channel);

        ChatLogManager.Instance.SendMessage(data);
    }

    public override IEnumerator StartBotAction(BotAI botAI, ActionRequest actionRequest)
    {
        BattleManager.Instance.SetPossibleTargetHighlights(actionRequest.actionCard, actionRequest);

        CardDisplay target = actionRequest.handTarget;
        yield return botAI.MoveCursor(target.transform.position);

        SelectHandTarget(actionRequest);
        yield return null;
    }
}
