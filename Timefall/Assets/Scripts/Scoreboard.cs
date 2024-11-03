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

    public void UpdateRound(int round, int[] cycleArr)
    {
        rows[round-1].SetUI(cycleArr);
    }

    public void UpdateBoard(int[] boardArr)
    {
        rows[8].SetUI(boardArr);
    }

    public void SetEndOfGameUI(int[] boardArr)
    {
        for (int i = 1; i < 8; i++)
        {
            int[] cycleArr = boardManager.CalculateVPForTurnCycle(i);
            UpdateRound(i, cycleArr);
        }

        UpdateBoard(boardArr);
    }

    public void SetCycleHighlight(int round)
    {
        // Debug.Log("Setting highlight for: " + round.ToString());
        int index = round -1;
        if (index - 1 >= 0)
        {
            rows[index-1].ToggleLabelHighlight();
        }

        rows[index].ToggleLabelHighlight();
    }

    public void SetCycleWinner(int round, Faction faction)
    {
        Debug.Log("Setting Winner for round: " + round.ToString());

        rows[round-1].ShowWinner(faction);
    }
}
