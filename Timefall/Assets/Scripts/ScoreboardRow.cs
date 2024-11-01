using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreboardRow : MonoBehaviour
{

    public TMP_Text stewardVPText;
    public TMP_Text seekerVPText;
    public TMP_Text sovereignVPText;
    public TMP_Text weaverVPText;
    // Start is called before the first frame update
    void Start()
    {
        // stewardVPText.text = "0";
        // seekerVPText.text = "0";
        // sovereignVPText.text = "0";
        // weaverVPText.text = "0";
    }

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
}
