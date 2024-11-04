using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapEssenceAction : EssenceAction
{
    public override bool CanTargetSpace(BoardSpace boardSpace)
    {
        Debug.Log("SwapEA: CanTargetSpace");

        EventCard eventCard = (EventCard) boardSpace.eventCard;
        AgentCard agentCard = boardSpace.agentCard;
        
        //must have an event & not have an agent
        if(eventCard == null || agentCard != null) { return false ;}

        return true;
    }

    /* [SWAP]
     * 1. Targetable Space Criteria:
     *      - Has an Event
     *      - Doesnt have an agent
     * 2. Atleast 2 Targetable Spaces
     *      - if not, return empty list
    */
    public override List<BoardSpace> GetTargatableSpaces(List<BoardSpace> spacesToTest)
    {
        Debug.Log("SwapEA: GetTargatableSpaces");

        List<BoardSpace> targetableSpaces = new List<BoardSpace>();

        foreach (BoardSpace boardSpace in spacesToTest)
        {
            if(!CanTargetSpace(boardSpace)) { continue;}

            targetableSpaces.Add(boardSpace);
        }

        //Must have atleast 2 plable spaces on board to swap
        if(targetableSpaces.Count < 2)
        {
            targetableSpaces.Clear();
        }

        Debug.Log(string.Format("SwapEA: PS Count [{0}]", targetableSpaces.Count));

        return targetableSpaces;
    }
}
