using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New CONVERT Essence Action", menuName = "Essence Action/CONVERT")]
[System.Serializable]
public class ConvertEssenceAction : EssenceAction
{
    public Texture CONVERT_TEX;
    public Texture2D CURSOR_CONVERT_TEX;

    public override bool CanBePlayed(ActionRequest actionRequest)
    {
        return actionRequest.potentialDiscardedTargets.Count >= 1; 
    }

    bool CanTargetDiscardedCard(Card discardCard, Faction requestFaction)
    { 
        //must be an agent & a different factions
        if(discardCard.GetCardType() != CardType.AGENT || discardCard.GetFaction() == requestFaction)
        {
            return false;
        }

        return true;
    }
    public override List<BoardSpace> GetTargatableSpaces(ActionRequest actionRequest)
    {
        return new List<BoardSpace>();
    }

    public override List<CardDisplay> GetTargatableHandDisplays(ActionRequest actionRequest)
    {
        return new List<CardDisplay>();
    }

    public override List<Card> GetTargatableDiscardedCards(ActionRequest actionRequest)
    {
        List<Card> targetableDiscards = new List<Card>();

        foreach (Card discardCard in actionRequest.potentialDiscardedTargets)
        {
            if(!CanTargetDiscardedCard(discardCard, actionRequest.player.faction)) { continue;}

            targetableDiscards.Add(discardCard);
        }

        return targetableDiscards;
    }

    public override Texture GetSelectionTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeDiscardedTargets.Count == 0)
        {
            return CONVERT_TEX;
        } 
        return null;
    }

    Texture2D GetCursorTexture(ActionRequest actionRequest)
    {
        if(actionRequest.activeDiscardedTargets.Count == 0)
        {
            return CURSOR_CONVERT_TEX;
        } 
        return null;
    }

    public override void SelectBoardTarget(ActionRequest actionRequest)
    {
        return;
    }

    public override void SelectHandTarget(ActionRequest actionRequest)
    {
        return;
    }

    public override void SelectDiscardedTarget(ActionRequest actionRequest)
    {
        List<Card> activeDiscardedTargets = actionRequest.activeDiscardedTargets;

        if(activeDiscardedTargets.Count != 0)
        {
            return;
        } 

        Card discardedTarget = actionRequest.discardedTarget;

        activeDiscardedTargets.Add(discardedTarget);

        Cursor.SetCursor(GetCursorTexture(actionRequest), Vector2.zero, CursorMode.Auto);

        Convert(discardedTarget, actionRequest.player, actionRequest);

        return;
    }

    public override void SetActionRequest(ActionRequest actionRequest)
    {
        actionRequest.doDiscard = true;
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

        //close discard display
        BattleManager.Instance.ClearPossibleTargetHighlights(actionRequest);

        hand.RemoveCardAfterPlaying(true,true);
        
        if(actionRequest.isBot)
        {
            actionRequest.player.EndBotAction();
        }
    }

    private void Convert(Card discardTarget, Player requestPlayer, ActionRequest actionRequest)
    {
        Hand.Instance.SetHandState(HandState.ACTION_START);
        
        AgentCard agent = (AgentCard) discardTarget;
        BattleManager.Instance.ConvertAgent(agent, requestPlayer.faction);

        SendChatLogMessage(requestPlayer, agent.data);

        AudioManager.Instance.Play(audioClip);

        //end of action
        EndAction(actionRequest);
    }

    void SendChatLogMessage(Player player, CardData agentData)
    {
        ChatMessageData data = new ChatMessageData(player, ChatMessageData.Action.Convert);

        data.cards.Add(agentData);

        ChatLogManager.Instance.SendMessage(data);
    }

    public override IEnumerator StartBotAction(BotAI botAI, ActionRequest actionRequest)
    {
        BattleManager.Instance.SetPossibleTargetHighlights(actionRequest.actionCard, actionRequest);

        Card agentToConvert = actionRequest.discardedTarget;

        CardDisplay discardedTarget = DiscardPileManager.Instance.GetCardDisplayForDiscardedCard(agentToConvert);
        
        yield return botAI.MoveCursor(discardedTarget.gameObject.transform.position);

        SelectDiscardedTarget(actionRequest);
        yield return null;
    }
}
