using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public Faction faction;
    public Deck deck;

    public List<Card> channelList = new List<Card>();

    public int handSize = 5;

    public PlayerEffect playerEffect;

    public bool isBot = false;

    [SerializeField]
    public BotAI botAI = null;

    void Awake()
    {
        if(isBot)
        {
            botAI = new BotAI();
            botAI.InitializeBot(this);
        }
    }

    public Color GetFactionColor()
    {
        return BattleManager.GetFactionColor(faction);
    }

    public void ChannelCard(CardDisplay cardDisplay)
    {
        cardDisplay.ApplyChannelEffect();

        channelList.Add(cardDisplay.displayCard);
    }

    public void ConvertAgent(AgentCard agentToConvert)
    {
        Hand.Instance.AddCardToHand(agentToConvert);
        //TODO: change ui?
    }

    public void ReviveAgent(AgentCard agentToRevive)
    {
        deck.discardPile.Remove(agentToRevive);
        Hand.Instance.AddCardToHand(agentToRevive);
    }

    public void OvertakeNextTurn(Player overtakePlayer)
    {
        if(playerEffect != null) {return;}

        playerEffect = new PlayerEffect(PlayerEffectType.OVERTAKE, overtakePlayer);
    }

    public void PuppetNextTurn(Player puppetPlayer)
    {
        if(playerEffect != null) {return;}

        playerEffect = new PlayerEffect(PlayerEffectType.PUPPET, puppetPlayer);
    }

    public void EndBotAction()
    {
        botAI.EndAction();
    }
}
