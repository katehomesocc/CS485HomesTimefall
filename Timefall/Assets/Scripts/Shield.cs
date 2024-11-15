using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
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
            targetSpace.AgentShieldExpired();
        }

        // if(spaceTargeted)
        // {
            
        // }
        //TODO: animation
        Destroy(this);
    }

    public void StartOfTurn()
    {
        switch (expiration)
        {
            case Expiration.NONE:
                return;
            case Expiration.NEXT_TURN:
                Expire();
            break;
            default:
                return;
        }
    }
}
