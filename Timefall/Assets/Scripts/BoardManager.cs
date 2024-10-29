using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    public static string LOCATION = "BOARD";

    public BoardSpace[] spaces = new BoardSpace[32];

    public GameManager gameManager;

    public int round = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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
        BoardSpace space = spaces[round-1];
        StartCoroutine(cardDisplay.ScaleToPositionAndSize(space.transform.position,space.transform.lossyScale, 1f, space.transform));

        if(cardDisplay.displayCard.cardType == CardType.EVENT)
        {
            space.SetEventCard((EventCard) cardDisplay.displayCard);
        }
        
        cardDisplay.playState = CardPlayState.IDK;
    }

    public int[] TotalVictoryPointsOnBoard()
    {
        int stewardsVP = 0;
        int seekersVP = 0;
        int sovereignsVP = 0;
        int weaversVP = 0;

        foreach (BoardSpace boardSpace in spaces)
        {
            EventCard eventCard = (EventCard) boardSpace.eventCard;

            if(eventCard == null) { continue;}

            stewardsVP += eventCard.victoryPoints[0];
            seekersVP += eventCard.victoryPoints[1];
            sovereignsVP += eventCard.victoryPoints[2];
            weaversVP += eventCard.victoryPoints[3];
        }

        return new int[] {stewardsVP, seekersVP, sovereignsVP, weaversVP};
    }


}
