using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield
{
    Player owner;
    Expiration expiration = Expiration.NONE;

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
            Debug.Log("SHIELD: agent targeted");
            targetSpace.AgentShieldExpired();
        }

        // if(spaceTargeted)
        // {
            
        // }
        //TODO: animation
        //Destroy(this);
    }

    public void StartOfTurn()
    {
        switch (expiration)
        {
            case Expiration.NONE:
                Debug.Log("SHIELD: Expiration.NONE");
                return;
            case Expiration.NEXT_TURN:
                if(owner != TurnManager.Instance.GetCurrentPlayer()){
                    Debug.Log("SHIELD: Expiration.NEXT_TURN");
                    return;
                }   
                Debug.Log("SHIELD: Expiring now :(");
                Expire();
                break;
            default:
                return;
        }
    }
}
