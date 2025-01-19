using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public static string LOCATION = "BOARD";

    public BoardSpace[] spaces = new BoardSpace[32];

    public List<BoardSpace> targetsAvailable = new List<BoardSpace>();


    public int round = 0;

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public void UnlockSpace(int _round, Faction faction)
    {
        round = _round;
        Color color = BattleManager.GetFactionColor(faction);
        spaces[round-1].Unlock(color);
    }

    public void PlaceTimelineEventForTurn(CardDisplay cardDisplay)
    {
        if(cardDisplay.displayCard.data.cardType != CardType.EVENT){ return;}

        BoardSpace space = spaces[round-1];

        space.PlaceEventOn((EventCardDisplay) cardDisplay);
    }

    public int[] CalculateVPInList(BoardSpace[] spacesToCalc)
    {
        int stewardsVP = 0;
        int seekersVP = 0;
        int sovereignsVP = 0;
        int weaversVP = 0;

        foreach (BoardSpace boardSpace in spacesToCalc)
        {
            EventCard eventCard = (EventCard) boardSpace.eventCard;

            if(eventCard == null) { continue;}

            stewardsVP += eventCard.eventCardData.victoryPoints[0];
            seekersVP += eventCard.eventCardData.victoryPoints[1];
            sovereignsVP += eventCard.eventCardData.victoryPoints[2];
            weaversVP += eventCard.eventCardData.victoryPoints[3];
        }

        Debug.Log(string.Format("BM calculated: [{0}] [{1}] [{2}] [{3}]", stewardsVP, seekersVP, sovereignsVP, weaversVP));

        return new int[] {stewardsVP, seekersVP, sovereignsVP, weaversVP};
    }

    public int[] TotalVictoryPointsOnBoard()
    {
        return CalculateVPInList(spaces);
    }

    public int[] CalculateVPForRound(int roundNumber)
    {
        int offset = (roundNumber - 1) * 4;

        Debug.Log(string.Format("roundNum:[{0}], offset:[{1}], calcuating: [{2},{3},{4},{5}]", roundNumber, offset, 0 + offset, 1 + offset, 2 + offset, 3 + offset));

        BoardSpace[] spacesToCalc = new BoardSpace[4];

        spacesToCalc[0] = spaces[0 + offset];
        spacesToCalc[1] = spaces[1 + offset];
        spacesToCalc[2] = spaces[2 + offset];
        spacesToCalc[3] = spaces[3 + offset];

        return CalculateVPInList(spacesToCalc);
    }

    public void SetPossibleTargetHighlight(Card card, ActionRequest actionRequest)
    {
        //For each space
            //Can card be played
                //if so highlight
            switch(card.GetCardType()) 
            {
                case CardType.AGENT:
                    SetAgentPossibilities((AgentCard) card, actionRequest);
                    break;
                case CardType.ESSENCE:
                    SetEssencePossibilities((EssenceCard) card, actionRequest);
                    break;
                case CardType.EVENT:


                    break;
                default:
                //Error handling
                    Debug.LogError("Invalid Card Type: " + card.data.cardType);
                    return;
            }

    }

    public List<BoardSpace> GetPossibleTargets(Card card, ActionRequest actionRequest)
    {
        Debug.Log(string.Format("BM GPT: {0}", actionRequest));
        switch(card.GetCardType()) 
        {
            case CardType.AGENT:
                return GetAgentPossibilities((AgentCard) card, actionRequest);
            case CardType.ESSENCE:
                return GetEssencePossibilities((EssenceCard) card, actionRequest);
            case CardType.EVENT:
                break;
            default:
            //Error handling
                Debug.LogError("Invalid Card Type: " + card.data.cardType);
                break;
        }
        return null;
    }

    public List<BoardSpace> GetEssencePossibilities(EssenceCard essenceCard, ActionRequest actionRequest)
    {   
        actionRequest.potentialBoardTargets = new List<BoardSpace>(spaces);
        return essenceCard.GetTargatableSpaces(actionRequest);
    }

    void SetEssencePossibilities(EssenceCard essenceCard, ActionRequest actionRequest)
    {
        List<BoardSpace> targetable = GetEssencePossibilities(essenceCard, actionRequest);

        foreach (BoardSpace boardSpace in targetable)
        {
            boardSpace.Highlight();
            targetsAvailable.Add(boardSpace);
            boardSpace.isTargetable = true;
        }
    }

    public List<BoardSpace> GetAgentPossibilities(AgentCard agentCard, ActionRequest actionRequest)
    {
        actionRequest.potentialBoardTargets = new List<BoardSpace>(spaces);
        return agentCard.GetTargatableSpaces(actionRequest);
    }

    void SetAgentPossibilities(AgentCard agentCard, ActionRequest actionRequest)
    {
        List<BoardSpace> targetable = GetAgentPossibilities(agentCard, actionRequest);

        foreach (BoardSpace boardSpace in targetable)
        {
            boardSpace.Highlight();
            targetsAvailable.Add(boardSpace);
            boardSpace.isTargetable = true;
        }
    }

    public void ClearPossibleTargetHighlights()
    {
        foreach (BoardSpace boardSpace in targetsAvailable)
        {
            boardSpace.EndHighlight();
            boardSpace.isTargetable = false;
        }

        targetsAvailable.Clear();
    }

    public void ResolveStartOfTurnOnBoard()
    {
        foreach (BoardSpace boardSpace in spaces)
        {
            boardSpace.ResolveStartOfTurn();
        }
    }

    public void ResolveEndOfTurnOnBoard()
    {
        foreach (BoardSpace boardSpace in spaces)
        {
            boardSpace.ResolveEndOfTurn();
        }
    }

}