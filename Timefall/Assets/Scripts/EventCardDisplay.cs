using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventCardDisplay : CardDisplay
{
    public List<Card> eventDB;
    public int pos = 0;
    public Image borderImage;

    [Header("Victory Points")]
    public TMP_Text stewardText;
    public Image stewardImage;
    public TMP_Text seekerText;
    public Image seekerImage;
    public TMP_Text sovereignText;
    public Image sovereignImage;
    public TMP_Text weaverText;
    public Image weaverImage;

    // Start is called before the first frame update
    void Start()
    {
        eventDB = GameObject.FindGameObjectWithTag("EventDB").GetComponent<CardDatabase>().cardList;
        displayCard = eventDB[0];
        ResetDisplay((EventCard) displayCard);
    }

    void ResetDisplay(EventCard eventCard) 
    {
        //base card
        nameText.text = eventCard.cardName;
        descText.text = eventCard.description;

        image.texture = eventCard.image;

        
        //victory points
        SetVictoryPoints(eventCard.victoryPoints);
    
        SetFactionColors(GetFactionColor(eventCard.faction));
        
    }

    void SetCard(EventCard eventCard)
    {
        displayCard = eventCard;
        ResetDisplay(eventCard);
    }

    void SetFactionColors(Color color)
    {
        borderImage.color = color;
    }

    void SetVictoryPoints(int[] vpArr)
    {
        // stewardText.text = GetVPText(vpArr[0]);
        // seekerText.text = GetVPText(vpArr[1]);
        // sovereignText.text = GetVPText(vpArr[2]);
        // weaverText.text = GetVPText(vpArr[3]);

        setVictoryPointUI(stewardText, stewardImage, vpArr[0]);
        setVictoryPointUI(seekerText, seekerImage, vpArr[1]);
        setVictoryPointUI(sovereignText, sovereignImage, vpArr[2]);
        setVictoryPointUI(weaverText, weaverImage, vpArr[3]);
    }

    string GetVPText(int vp)
    {
        string symbol = ""; 
        if(vp > 0) { symbol = "+";}
        else if (vp < 0) { symbol = "-";}
        else {return "";}

        return string.Format("{0}{1}", symbol, Mathf.Abs(vp));
    }

    void setVictoryPointUI(TMP_Text vpTMP, Image vpImage, int vp)
    {
        string vpText = GetVPText(vp);
        if (vpText.Equals(""))
        { //black out VP for no points add/removed
            vpTMP.enabled = false;
            vpImage.enabled = false;
        } else 
        { //set VP text
            vpTMP.enabled = true;
            vpTMP.text = vpText;
            vpImage.enabled = true;
        }
    }

    public void ShowNextCard()
    {
        if(pos < eventDB.Count-1)
        {
            pos++;
        } 
        else
        {
            pos = 0;
        }

        SetCard((EventCard) eventDB[pos]);

    }

}
