using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ResearchCardDisplay : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    public CardData data;

    [Header("UI")]
    public RawImage selectionImage;
    public TMP_Text nameText;
    public TMP_Text descText;
    public RawImage image;
    public GameObject expandBackground;

    void Start()
    {
        if(data != null)
        {
            nameText.text = data.cardName;
            descText.text = data.description;
            image.texture = data.image;
        }

        // HighlightOff();
        
    }

    public void InstantiateInInventory(Transform inventoryParent)
    {
        transform.SetParent(inventoryParent, false);

        // transform.localScale =  new Vector3(0.55f, 0.55f, 0.55f);

    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        // if(!inHand && !onBoard){ return;}

        // HighlightOn();

        // battleManager.ExpandCardView(displayCard, true);
        // isExpanded = true;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // if(!inHand && !onBoard){ return;}
        
        // HighlightOff();

        // battleManager.CloseExpandCardView();
        // isExpanded = false;
    }

    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {

    }

    public CardType GetCardType()
    {
        return this.data.cardType;
    }

    public void HighlightOn()
    {
        selectionImage.enabled = true;
    }

    public void HighlightOff()
    {
        selectionImage.enabled = false;
    }

    public int GetPositionInParent()
    {

        return this.transform.GetSiblingIndex();
    }

    public void AddToContentPanel(Transform parentTransform)
    {
        this.transform.SetParent(parentTransform, false);
    }

}
