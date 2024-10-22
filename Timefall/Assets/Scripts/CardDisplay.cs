using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static Color COLOUR_SEEKERS = new Color(33f/255,197f/255,104f/255, 1f);
    public static Color COLOUR_SOVEREIGNS = new Color(255f/255,35f/255,147f/255, 1f);
    public static Color COLOUR_STEWARDS = new Color(24f/255,147f/255,248f/255, 1f);
    public static Color COLOUR_WEAVERS = new Color(97f/255,65f/255,172f/255, 1f);
    public Card displayCard;

    public TMP_Text nameText;
    public TMP_Text descText;
    public RawImage image;

    public Transform returnParent = null;
    int returnSiblingIndex = 0;
    public bool placing = false;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = displayCard.cardName;
        descText.text = displayCard.description;

        image.texture = displayCard.image;
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

        returnParent = this.transform.parent;
        returnSiblingIndex = this.transform.GetSiblingIndex();

        this.transform.SetParent(this.transform.parent.parent); //edit layer later

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");

        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("OnEndDrag");

        this.transform.SetParent(returnParent);   
        this.transform.SetSiblingIndex(returnSiblingIndex);

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void Place(Transform droppedParent)
    {
        if(droppedParent != returnParent)
        {
            returnParent = droppedParent;
            returnSiblingIndex = returnParent.childCount;
        }
    }

}
