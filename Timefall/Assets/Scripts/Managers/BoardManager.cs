using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    public static string LOCATION = "BOARD";

    public BoardSpace[] spaces = new BoardSpace[32];

    public List<BoardSpace> targetsAvailable = new List<BoardSpace>();


    public int round = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnlockSpace(int _round, Faction faction)
    {
        round = _round;
        Color color = CardDisplay.GetFactionColor(faction);
        spaces[round-1].Unlock(color);
    }

    public void PlaceTimelineEventForTurn(CardDisplay cardDisplay)
    {
        if(cardDisplay.displayCard.data.cardType != CardType.EVENT){ return;}

        BoardSpace space = spaces[round-1];

        StartCoroutine(cardDisplay.ScaleToPositionAndSize(space.transform.position,space.transform.lossyScale, 1f, space.transform));
        
        space.SetEventCard((EventCardDisplay)cardDisplay);
        
        cardDisplay.playState = CardPlayState.IDK;
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

        return new int[] {stewardsVP, seekersVP, sovereignsVP, weaversVP};
    }

    public int[] TotalVictoryPointsOnBoard()
    {
        return CalculateVPInList(spaces);
    }

    public int[] CalculateVPForTurnCycle(int cycleNumber)
    {
        int offset = (cycleNumber - 1) * 4;

        Debug.Log(string.Format("cycleNum:[{0}], offset:[{1}], calcuating: [{2},{3},{4},{5}]", cycleNumber, offset, 0 + offset, 1 + offset, 2 + offset, 3 + offset));

        BoardSpace[] spacesToCalc = new BoardSpace[4];

        spacesToCalc[0] = spaces[0 + offset];
        spacesToCalc[1] = spaces[1 + offset];
        spacesToCalc[2] = spaces[2 + offset];
        spacesToCalc[3] = spaces[3 + offset];

        return CalculateVPInList(spacesToCalc);
    }

    public void SetCardPossibilities(Card card)
    {
        //For each space
            //Can card be played
                //if so highlight
            switch(card.data.cardType) 
            {
                case CardType.AGENT:


                    break;
                case CardType.ESSENCE:
                    SetEssencePossibilities((EssenceCard) card);
                    break;
                case CardType.EVENT:


                    break;
                default:
                //Error handling
                    Debug.Log ("Invalid Card Type: " + card.data.cardType);
                    return;
            }



    }

    void SetEssencePossibilities(EssenceCard essenceCard)
    {
        List<BoardSpace> targetable = essenceCard.GetTargatableSpaces(new List<BoardSpace>(spaces));

        foreach (BoardSpace boardSpace in targetable)
        {
            boardSpace.Highlight();
            targetsAvailable.Add(boardSpace);
            boardSpace.isTargetable = true;
        }
    }

    public void ClearPossibilities()
    {
        foreach (BoardSpace boardSpace in targetsAvailable)
        {
            boardSpace.EndHighlight();
            boardSpace.isTargetable = false;
        }

        targetsAvailable.Clear();
    }

}