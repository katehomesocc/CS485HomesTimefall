using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public TurnManager turnManager;
    public BoardManager boardManager;
    public Hand hand;

    public bool autoplay = false;
    public int autoplayUntilTurn = 32;

    int turn = 1;
    public float autoplayWaitTime = 1.5f;

    void Awake()
    {
        turnManager = FindObjectOfType<TurnManager>();
        boardManager = FindObjectOfType<BoardManager>();
        hand = FindObjectOfType<Hand>();

        if(boardManager == null)
        {
            Debug.LogError("25: boardManager == null");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(autoplay)
        {
            StartCoroutine(QueueFirstAutoplay());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator QueueFirstAutoplay()
    {
        Debug.Log("Queuing First Autoplay");
        yield return new WaitForSeconds(autoplayWaitTime);

        StartCoroutine(AutoplayRound());
    }

    IEnumerator AutoplayRound()
    {
        Debug.Log("AutoplayRound");
        
        hand.AutoPlayTimelineCard();
        turnManager.EndTurn();

        yield return new WaitForSeconds(0.1f);

        if(turn < autoplayUntilTurn)
        {
            turn++;
            StartCoroutine(AutoplayRound());
        }
        
    }

    public void SetCardPossibilities(Card card)
    {
        boardManager.SetCardPossibilities(card);
    }

    public void ClearPossibilities()
    {
        if(boardManager == null)
        {
            Debug.LogError("81:boardManager == null");
            boardManager = FindObjectOfType<BoardManager>();
        }

        if(boardManager == null)
        {
            Debug.LogError("87:boardManager still == null");
        }
        boardManager.ClearPossibilities();
    }

}
