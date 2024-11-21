using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Faction faction;
    public Deck deck;

    public List<Card> channelList = new List<Card>();

    public int handSize = 5;

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
}
