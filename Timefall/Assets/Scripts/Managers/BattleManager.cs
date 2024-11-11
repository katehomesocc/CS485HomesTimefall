using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    TurnManager turnManager;
    BoardManager boardManager;
    Hand hand;

    public bool autoplay = false;
    public int autoplayUntilTurn = 32;

    int turn = 1;
    public float autoplayWaitTime = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        turnManager = TurnManager.Instance;
        boardManager = BoardManager.Instance;
        hand = Hand.Instance;

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
        boardManager.ClearPossibilities();
    }

}
