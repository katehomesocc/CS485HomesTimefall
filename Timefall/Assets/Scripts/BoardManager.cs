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
        cardDisplay.playState = CardPlayState.IDK;
    }


}
