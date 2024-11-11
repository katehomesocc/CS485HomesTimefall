using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{

    BoardManager boardManager;
    public ScoreboardRow[] rows = new ScoreboardRow[9];

    // Start is called before the first frame update
    void Start()
    {
        boardManager = BoardManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRound(int round, int[] roundArr)
    {
        ScoreboardRow row = rows[round-1];
        if(row.isUnlocked){
            row.SetUI(roundArr);
        }
        
    }

    public void UpdateBoard(int[] boardArr)
    {
        rows[8].SetUI(boardArr);
    }

    public void SetEndOfGameUI(int[] boardArr)
    {
        // for (int i = 1; i < 8; i++)
        // {
        //     int[] roundArr = boardManager.CalculateVPForRound(i);
        //     UpdateRound(i, roundArr);
        // }

        UpdateBoard(boardArr);
    }

    public void SetRoundHighlight(int round)
    {
        // Debug.Log("Setting highlight for: " + round.ToString());
        int index = round -1;
        if (index - 1 >= 0)
        {
            rows[index-1].ToggleLabelHighlight();
        }

        rows[index].ToggleLabelHighlight();
    }

    public void SetRoundWinner(int round, Faction faction)
    {
        Debug.Log("Setting Winner for round: " + round.ToString());

        ScoreboardRow row = rows[round-1];
        row.ShowWinner(faction);
        row.isUnlocked = false;
    }
}
