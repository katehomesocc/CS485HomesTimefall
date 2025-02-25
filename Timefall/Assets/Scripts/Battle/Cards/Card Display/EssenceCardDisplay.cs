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

        if(actionRequest == null)
        {
            Debug.Log("ECDisplay.SetCard: actionRequest is null");
            return;
        }

        if(essenceCard == null)
        {
            Debug.Log("ECDisplay.SetCard: essenceCard  is null");
            return;
        }

        essenceCard.SetActionRequest(actionRequest);
    }

    public EssenceCard GetEssenceCard()
    {
        return (EssenceCard) displayCard;
    }

    public override ActionType GetActionType()
    {
        return GetEssenceCard().GetActionType();
    }

    public void PlayFromHand(bool isBot)
    {
        Vector3 newPosition = this.transform.localPosition + new Vector3(0, 50, 0);
        StartCoroutine(this.MoveToLocalPosition(newPosition, 0.25f));

        if(isBot)
        {
            StartCoroutine(GetEssenceCard().StartBotAction(actionRequest));
            return;
        }
                    
        GetEssenceCard().StartAction(actionRequest);
    }

}