using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shield
{
    public Player owner;
    public Expiration expiration = Expiration.NONE;

    BoardSpace targetSpace;

    bool spaceTargeted = false;
    bool agentTargeted = false;

    public Shield()
    {

    }

    public Shield(Player own, Expiration exp, BoardSpace space, bool doSpace, bool doAgent)
    {
        this.owner = own;
        this.expiration = exp;
        this.targetSpace = space;
        spaceTargeted = doSpace;
        agentTargeted = doAgent;
    }

    public void Expire()
    {
        if(agentTargeted)
        {
            Debug.Log("SHIELD EXPIRED: agent targeted");
            targetSpace.AgentShieldExpired();
        }

        if(spaceTargeted)
        {
            Debug.Log("SHIELD EXPIRED: event targeted");
            targetSpace.EventShieldExpired();
        }
        //TODO: animation
        //Destroy(this);
    }

    public void StartOfTurn()
    {
        switch (expiration)
        {
            case Expiration.NONE:
                // Debug.Log("SHIELD: Expiration.NONE");
                return;
            case Expiration.NEXT_TURN:
                if(owner != TurnManager.Instance.GetCurrentPlayer()){
                    // Debug.Log("SHIELD: Expiration.NEXT_TURN");

                    if(agentTargeted)
                    {
                        // Debug.Log("SHIELD: agent targeted");
                        targetSpace.AgentShieldCountdown();
                    }

                    if(spaceTargeted)
                    {
                        // Debug.Log("SHIELD: event targeted");
                        targetSpace.EventShieldCountdown();
                    }
                    return;
                }   
                // Debug.Log("SHIELD: Expiring now :(");
                Expire();
                break;
            default:
                return;
        }
    }

    public void SubscribeToAgent(UnityEvent onAgentDeath)
    {
        if(onAgentDeath == null)
        {
            Debug.LogError("onAgentDeath == null");
        }
        onAgentDeath.AddListener(Expire);
    }
}
