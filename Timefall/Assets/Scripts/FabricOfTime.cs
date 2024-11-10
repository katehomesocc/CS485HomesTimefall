using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FabricOfTime : MonoBehaviour
{
    public List<FabricOfTimeRound> rounds = new List<FabricOfTimeRound>();
    public TMP_Text roundText;
    public GameObject fotPanel;

    public float endOfRoundTime = 5f;

    public void ShowPanel()
    {
        fotPanel.SetActive(true);
    }

    public void HidePanel()
    {
        fotPanel.SetActive(false);
    }

    public void SetRoundWinner(int round, Faction faction)
    {

        FabricOfTimeRound currentRound = rounds[round-1];
        currentRound.SetWinner(faction);
        
    }

    public IEnumerator PerformEndOfRoundUpdate(int round, Faction winner)
    {
        Debug.Log("FOT: PerformEndOfRoundUpdate...");
        roundText.text = string.Format("Round {0} Winner: {1}", round, winner);

        SetRoundWinner(round, winner);

        ShowPanel();

        yield return new WaitForSeconds(endOfRoundTime);

        HidePanel();
        roundText.text = "";

        yield return new WaitForSeconds(endOfRoundTime / 3f);

        Debug.Log("FOT: ...ending");    
    }

}
