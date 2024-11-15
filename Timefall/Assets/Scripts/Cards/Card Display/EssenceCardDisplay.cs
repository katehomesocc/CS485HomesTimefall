using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EssenceCardDisplay : CardDisplay
{

    private void ResetDisplay(EssenceCardData essenceCard) 
    {
        //base card
        nameText.text = essenceCard.cardName;
        descText.text = essenceCard.description;

        image.texture = essenceCard.image;
    
        // SetFactionColors(GetFactionColor(eventCard.faction));
        
    }

    public void SetCard(EssenceCard essenceCard)
    {
        displayCard = essenceCard;
        ResetDisplay(essenceCard.essenceCardData);
    }

    public EssenceCard GetEssenceCard()
    {
        return (EssenceCard) displayCard;
    }

    public EssenceCard PlayFromHand(Player player)
    {
        Vector3 newPosition = this.transform.position + new Vector3(0, 50, 0);
        StartCoroutine(this.MoveToPosition(newPosition, 0.25f));
        GetEssenceCard().StartAction(player);
        return GetEssenceCard();
    }

}