using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{

    public BoardManager boardManager;
    public ScoreboardRow[] rows = new ScoreboardRow[9];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEndOfGameUI()
    {
        for (int i = 0; i < 8; i++)
        {
            int[] cycleArr = boardManager.CalculateVPForTurnCycle(i+1);
            rows[i].SetUI(cycleArr);
        }

        int[] boardArr = boardManager.TotalVictoryPointsOnBoard();
        rows[8].SetUI(boardArr);
    }

    
    
}
