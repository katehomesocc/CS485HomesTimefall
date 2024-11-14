using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreboardRow : MonoBehaviour
{
    public TMP_Text labelText;
    public TMP_Text stewardVPText;
    public TMP_Text seekerVPText;
    public TMP_Text sovereignVPText;
    public TMP_Text weaverVPText;

    public GameObject winnerBox;
    public RawImage gemImage;

    public TMP_Text winnerText;

    bool isHighlighted = false;

    public bool isUnlocked = true;

    public void SetUI(int[] vpArr)
    {
        stewardVPText.text = GetVPText(vpArr[0]);
        seekerVPText.text = GetVPText(vpArr[1]);
        sovereignVPText.text = GetVPText(vpArr[2]);
        weaverVPText.text = GetVPText(vpArr[3]);
    }

    string GetVPText(int vp)
    {
        string symbol = ""; 
        if(vp > 0) { symbol = "+";}
        else if (vp < 0) { symbol = "-";}

        return string.Format("{0}{1}", symbol, Mathf.Abs(vp));
    }

    public void ToggleLabelHighlight()
    {
        if(isHighlighted)
        {
            labelText.color = Color.white;
            isHighlighted = false;
        } else
        {
            labelText.color = Color.yellow;
            isHighlighted = true;
        }
        
    }

    public void ShowWinner(Faction faction)
    {
        gemImage.color = BattleManager.GetFactionColor(faction);
        // winnerText.text = "+5";
        winnerBox.SetActive(true);
    }
}
