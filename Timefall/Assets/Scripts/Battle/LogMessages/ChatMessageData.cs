using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessageData
{
    public enum Action { TESTING, DeployAgent, PatchEvent, ReplaceEvent, SwapEvent }

    public Action action;
    public Player player;

    public List<BoardSpace> spaces = new List<BoardSpace>();
    public List<CardData> cards = new List<CardData>();
    
    public ChatMessageData(Player _player, Action _action)
    {
        player = _player;
        action = _action;
    }

    public string BuildMessageString()
    {
        switch (action)
        {
            case Action.DeployAgent:
                return DeployAgentMessage();
            case Action.PatchEvent:
                return PatchEventMessage();
            case Action.ReplaceEvent:
                return ReplaceEventMessage();
            case Action.SwapEvent:
                return SwapEventMessage();
            default:
                return "default message string";
        }
    }

    public bool SingleLineMessage()
    {
        switch (action)
        {
            case Action.ReplaceEvent:
                return false;
            default:
                return true;
        }
    }

    string DeployAgentMessage()
    {
        string playerText = PlayerText();
        string agentText = CardText(cards[0]);
        string eventText = CardText(cards[1]);

        return $"{playerText} deployed {agentText} on {eventText}.";
    }

    string PatchEventMessage()
    {
        string playerText = PlayerText();
        string agentText = CardText(cards[0]);
        string eventText = CardText(cards[1]);

        return $"{playerText} used {agentText} to patch a hole on the timeline with {eventText}.";
    }

    string ReplaceEventMessage()
    {
        string playerText = PlayerText();
        string originalEventText = CardText(cards[0]);
        string newEventText = CardText(cards[1]);

        return $"{playerText} replaced {originalEventText} with {newEventText}.";
    }

    string SwapEventMessage()
    {
        string playerText = PlayerText();
        string event1Text = CardText(cards[0]);
        string event2Text = CardText(cards[1]);

        return $"{playerText} swapped {event1Text} with {event2Text}.";
    }

    string PlayerText()
    {
        string playerColor = ColorUtility.ToHtmlStringRGB(player.GetFactionColor());
        string playerName = player.playerName;
        return $"<color=#{playerColor}><b>{playerName}</b></color>";
    }

    string CardText(CardData cardData)
    {
        Faction faction = cardData.faction;
        
        if(faction == Faction.NONE)
        {
            return $"<b>{cardData.cardName}</b>";
        }

        string factionColor = ColorUtility.ToHtmlStringRGB(BattleManager.GetFactionColor(faction));

        return $"<color=#{factionColor}><b>{cardData.cardName}</b></color>";
        
    }
}
