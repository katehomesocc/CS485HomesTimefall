using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class BoardSpace : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static string LOCATION = "BOARD";

    public RawImage border;

    public Color resetColor;

    public TurnManager turnManager;

    public GameObject highlight;

    [Header("Display Prefabs")]
    public GameObject agentDisplayPrefab;
    public GameObject essenceDisplayPrefab;
    public GameObject eventDisplayPrefab;

    [Header("Properties")]
    public bool isUnlocked = false;
    public bool hasEvent = false;
    public bool hasAgent = false;
    public bool isTargetable = false;
    public EventCard eventCard;

    public AgentCard agentCard;

    public EventCardDisplay eventDisplay;
    public AgentCardDisplay agentDisplay;

    // Start is called before the first frame update
    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        border.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isUnlocked && isTargetable)
        {
            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

            CardDisplay display = eventData.pointerDrag.GetComponent<CardDisplay>();

            if (display == null) {return;}

            if (display.onBoard) {return;}

            display.inPlaceAnimation = true;

            PlaceCard(display);
        }
    }

    void PlaceCard(CardDisplay cardDisplay) 
    {
        StartCoroutine(cardDisplay.ScaleToPositionAndSize(this.transform.position, this.transform.lossyScale, 1f, this.transform));
        
        SetCardDisplay(cardDisplay.displayCard);
        
        cardDisplay.playState = CardPlayState.IDK;
    }


    public void SetCardDisplay(Card card)
    {
        if(card == null){ return;}

        CardType cardType = card.cardType;

        Debug.Log (card.ToString());

        GameObject obj = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                // obj = Instantiate(agentDisplayPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                // AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                // if(agentDC == null){return;} 
                // agentDC.SetCard(card);
                // agentDC.Place(this.transform, LOCATION);

                // break;
                return;
            case CardType.ESSENCE:
                // obj = Instantiate(essenceDisplayPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                // EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

                // if(essenceDC == null){return;} 
                // essenceDC.SetCard(card);
                // essenceDC.Place(this.transform, LOCATION);
                // break;
                return;
            case CardType.EVENT:
                obj = Instantiate(eventDisplayPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return;} 
                eventDC.SetCard(card);
                eventDC.Place(this.transform, LOCATION);
                SetEventCard(eventDC);
                break;
            default:
            //Error handling
                Debug.Log ("Invalid Card Type: " + cardType);
                return;
        }

        obj.transform.SetParent(this.transform, false);

    }

    public void Unlock(Color color)
    {
        border.color = color;
        isUnlocked = true;
    }

        //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(isUnlocked)
        {
            // Highlight();
            
            if(pointerEventData.pointerDrag == null) { return;}

            CardDisplay display = pointerEventData.pointerDrag.GetComponent<CardDisplay>();

            if (display == null) {return;}


            if (display.onBoard) {return;}

            if(!CanPlayCardOnThisSpace(display.displayCard)) {return;}

            Card card = display.displayCard;

            Debug.Log("Can be played on this space");

            turnManager.SetVictoryPointUI();

        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(isUnlocked)
        {
            // EndHighlight();
        }
    }

    bool CanPlayCardOnThisSpace(Card card)
    {
        bool turnManagerApproved = turnManager.CanPlayCard(card);

        return turnManagerApproved;
        
    }

    public void SetEventCard(EventCardDisplay display)
    {
        this.eventDisplay = display;
        this.eventCard = (EventCard) eventDisplay.displayCard;
        this.hasEvent = true;
    }

    public void Highlight()
    {
        highlight.transform.SetAsLastSibling();
        highlight.SetActive(true);
        // resetColor = border.color;
        // border.color = Color.yellow;
    }

    public void EndHighlight()
    {
        highlight.SetActive(false);
        // border.color = resetColor;
    }

    
}
