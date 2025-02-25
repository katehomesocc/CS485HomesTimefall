using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EssenceRCD : ResearchCardDisplay
{
    private void ResetDisplay(EssenceCardData data) 
    {
        //base card
        nameText.text = data.cardName;
        descText.text = data.description;

        image.texture = data.image;
    
        // SetFactionColors(GetFactionColor(eventCard.faction));
        
    }

    public void SetCard(EssenceCardData essenceData)
    {
        ResetDisplay(essenceData);
    }

}
