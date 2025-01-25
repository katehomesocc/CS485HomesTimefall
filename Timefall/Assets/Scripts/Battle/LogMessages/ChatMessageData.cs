using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessageData
{
    public enum Action { TESTING, ReplaceEvent }

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
            case Action.ReplaceEvent:
                return ReplaceEventMessage();
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

    string ReplaceEventMessage()
    {
        string playerText = PlayerText();
        string originalEventText = EventText(cards[0]);
        string newEventText = EventText(cards[1]);

        return $"{playerText} replaced {originalEventText} with {newEventText}.";
    }

    string PlayerText()
    {
        string playerColor = ColorUtility.ToHtmlStringRGB(player.GetFactionColor());
        string playerName = player.playerName;
        return $"<color=#{playerColor}><b>{playerName}</b></color>";
    }

    string EventText(CardData cardData)
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
