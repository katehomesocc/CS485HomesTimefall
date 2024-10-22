using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static Color COLOUR_SEEKERS = new Color(33f/255,197f/255,104f/255, 1f);
    public static Color COLOUR_SOVEREIGNS = new Color(255f/255,35f/255,147f/255, 1f);
    public static Color COLOUR_STEWARDS = new Color(24f/255,147f/255,248f/255, 1f);
    public static Color COLOUR_WEAVERS = new Color(97f/255,65f/255,172f/255, 1f);
    public Card displayCard;
    public RawImage selectionImage;

    public TMP_Text nameText;
    public TMP_Text descText;
    public RawImage image;

    public Transform returnParent = null;
    int returnSiblingIndex = 0;
    public bool inHand = false;
    public bool onBoard = false;
    public bool isExpanded = false;
    public Hand lastHand;

    // Start is called before the first frame update
    void Start()
    {
        if(displayCard != null)
        {
            nameText.text = displayCard.cardName;
            descText.text = displayCard.description;
            image.texture = displayCard.image;
        }

        selectionImage.enabled = false;
        
    }

    public Color GetFactionColor(Faction faction)
    {
        switch(faction) 
        {
            case Faction.WEAVERS:
                return COLOUR_WEAVERS;
            case Faction.SEEKERS:
                return COLOUR_SEEKERS;
            case Faction.SOVEREIGNS:
                return COLOUR_SOVEREIGNS;
            case Faction.STEWARDS:
                return COLOUR_STEWARDS;
        }

        return Color.black;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("OnBeginDrag");

        if(!inHand){ return;}

        returnParent = this.transform.parent;
        returnSiblingIndex = this.transform.GetSiblingIndex();

        this.transform.SetParent(this.transform.parent.parent); //edit layer later

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(string.Format("OnDrag | inHand = {0} | onBoard = {1} ", inHand, onBoard));

        if(!inHand){ return;}

        if(isExpanded)
        {
            lastHand.CloseExpandCardView();
            isExpanded = false;
        }

        this.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("OnEndDrag");

        // if(onBoard){ return;}

        this.transform.SetParent(returnParent);   
        this.transform.SetSiblingIndex(returnSiblingIndex);

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void Place(Transform droppedParent, string location)
    {
        if(droppedParent == null || droppedParent != returnParent)
        {
            if(droppedParent != null)
            {
                returnParent = droppedParent;
                returnSiblingIndex = returnParent.childCount;
            }
            
            inHand = false;
            onBoard = false;
            switch(location) 
            {
                case "HAND":
                    inHand = true;
                    lastHand = GetComponentInParent<Hand>();
                    break;
                case "BOARD":
                    onBoard = true;
                    break;
            }

            Debug.Log (string.Format("Placing At: {0} | inHand = {1} | onBoard = {2} ", location, inHand, onBoard));
        }
    }

    public void InstantiateInHand(Transform handParent)
    {
        transform.SetParent(handParent, false);

        transform.localScale =  new Vector3(0.55f, 0.55f, 0.55f);

        Place(handParent, "HAND");

        
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(!inHand){ return;}

        selectionImage.enabled = true;

        Hand hand = GetComponentInParent<Hand>();
        if(hand == null){ return;}

        hand.ExpandCardView(displayCard);
        isExpanded = true;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(!inHand){ return;}
        
        selectionImage.enabled = false;

        Hand hand = GetComponentInParent<Hand>();
        if(hand == null){ return;}

        hand.CloseExpandCardView();
        isExpanded = false;
    }


}
