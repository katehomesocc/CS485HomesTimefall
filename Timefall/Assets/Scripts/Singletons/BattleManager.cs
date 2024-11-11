using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    TurnManager turnManager;
    BoardManager boardManager;
    Hand hand;

    public bool autoplay = false;
    public int autoplayUntilTurn = 32;

    int turn = 1;
    public float autoplayWaitTime = 1.5f;

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
        yield return turnManager.EndTurn();

        yield return new WaitForSeconds(0.1f);

        if(turn < autoplayUntilTurn)
        {
            turn++;
            StartCoroutine(AutoplayRound());
        }
        
    }
}
