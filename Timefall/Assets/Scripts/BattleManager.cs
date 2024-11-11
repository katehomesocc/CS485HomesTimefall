using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    TurnManager turnManager;
    BoardManager boardManager;
    Hand hand;

    int turn = 1;

    [Header("Autoplay (Development Testing)")]
    public bool autoplay = false;
    public int autoplayUntilTurn = 32;
    public float autoplayWaitTime = 1.5f;

    [Header("Start Of Game")]
    public int startupTime = 5;
    public GameObject startOfGamePanel;
    public TMP_Text startOfGameText;

    public TMP_Text startOfGameCountdownText;

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

        StartCoroutine(StartOfGame());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator QueueFirstAutoplay()
    {
        Debug.Log("BM: waiting for TM.StartOFGame()...");
        

        yield return StartCoroutine(turnManager.StartOfGame());

        Debug.Log("BM: ...Queuing First Autoplay");

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

    IEnumerator StartOfGame()
    {
        //TODO: play audio?
        
        startOfGamePanel.SetActive(true);

        for (int i = startupTime; i > 0; i--)
        {
            startOfGameCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
            
        }

        startOfGameText.text = "";

        startOfGameCountdownText.text = "BATTLE";
        yield return new WaitForSeconds(1);

        startOfGamePanel.SetActive(false);

        if(autoplay)
        {
            StartCoroutine(QueueFirstAutoplay());
        } else
        {
            turnManager.StartGame();
        }
    }
}
