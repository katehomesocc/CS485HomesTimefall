using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessageData
{
    public enum Action { TESTING, AgentShield, Channel, Convert, CosmicBlast, //
                    DeployAgent, PatchEvent, Paradox, PlayerShield, Revive, //
                    ReplaceEvent, SwapEvent } //

    public Action action;
    public Player player;
    public List<CardData> cards = new List<CardData>();

    public Expiration expiration;
    
    public ChatMessageData(Player _player, Action _action)
    {
        player = _player;
        action = _action;
    }

    public string BuildMessageString()
    {
        switch (action)
        {
            case Action.AgentShield:
                return AgentShieldMessage();
            case Action.Channel:
                return ChannelMessage();
            case Action.Convert:
                return ConvertMessage();
            case Action.CosmicBlast:
                return CosmicBlastMessage();
            case Action.DeployAgent:
                return DeployAgentMessage();
            case Action.PatchEvent:
                return PatchEventMessage();
            case Action.Paradox:
                return ParadoxMessage();
            case Action.PlayerShield:
                return PlayerShieldMessage();
            case Action.Revive:
                return ReviveMessage();
            case Action.ReplaceEvent:
                return ReplaceEventMessage();
            case Action.SwapEvent:
                return SwapEventMessage();
            default:
                return "default message string";
        }
    }

    string AgentShieldMessage()
    {
        string playerText = PlayerText();
        string agentText = CardText(cards[0]);
        string eventOrAgentText = CardText(cards[1]);
        string expText = ExpirationText(expiration, cards[0]);

        return $"{playerText} used {agentText} to cast a shield on {eventOrAgentText} that will expire {expText}.";
    }

    string ChannelMessage()
    {
        string playerText = PlayerText();

        return $"{playerText} is channeling.";
    }

    string ConvertMessage()
    {
        string playerText = PlayerText();
        string agentText = CardText(cards[0]);

        return $"{playerText} has converted {agentText}.";
    }
    string CosmicBlastMessage()
    {
        string playerText = PlayerText();

        if(cards.Count == 1)
        {
            string agentText = CardText(cards[0]);
            return $"{playerText} cast a Cosmic Blast on {agentText}.";
        }

        string castingAgentText = CardText(cards[0]);
        string blastAgentText = CardText(cards[1]);

        return $"{playerText} used {castingAgentText} to cast a Cosmic Blast on {blastAgentText}.";
    }

    string PlayerShieldMessage()
    {
        string playerText = PlayerText();
        string eventText = CardText(cards[0]);

        return $"{playerText} cast a shield on {eventText} that will expire on their next turn.";
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

    string ParadoxMessage()
    {
        string playerText = PlayerText();

        if(cards.Count == 1)
        {
            string eventText = CardText(cards[0]);
            return $"{playerText} cast a Paradox on {eventText}.";
        }

        string castingAgentText = CardText(cards[0]);
        string paradoxEventText = CardText(cards[1]);

        return $"{playerText} used {castingAgentText} to cast a Paradox on {paradoxEventText}.";
    }

    string ReviveMessage()
    {
        string playerText = PlayerText();

        if(cards.Count == 1)
        {
            string agentText = CardText(cards[0]);
            return $"{playerText} revived {agentText}.";
        }

        string castingAgentText = CardText(cards[0]);
        string reviveAgentText = CardText(cards[1]);

        return $"{playerText} used {castingAgentText} to revive {reviveAgentText}.";
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

    string ExpirationText(Expiration expiration, CardData agentData)
    {
        Faction faction = agentData.faction;

        string factionColor = ColorUtility.ToHtmlStringRGB(BattleManager.GetFactionColor(faction));

        string whenDamagedText = $"when <color=#{factionColor}><b>{agentData.cardName}</b></color> is damaged";

        if(expiration == Expiration.NONE)
        {
            return whenDamagedText;
        }

        return $"on the next <color=#{factionColor}><b>{faction.ToString()}</b></color> turn or {whenDamagedText}";
        
    }
}
