using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardSpace : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static string LOCATION = "BOARD";
    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;

    public bool isUnlocked = false;
    public bool hasEvent = false;
    public bool hasAgent = false;


    public Card testCard;

    public RawImage border;

    public Color resetColor;

    public TurnManager turnManager;
    
    // Start is called before the first frame update
    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        //SetCard(testCard);
        border.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isUnlocked)
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
        //cardDisplay.Place(this.transform, LOCATION);
        SetCard(cardDisplay.displayCard);

        hasEvent = true;//for testing, change

        StartCoroutine(cardDisplay.MoveToPosition(this.transform.position, 10f));

        //Destroy(cardDisplay.gameObject);
    }


    public void SetCard(Card card)
    {
        if(card == null){ return;}

        CardType cardType = card.cardType;

        Debug.Log (card.ToString());

        GameObject obj = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                if(agentDC == null){return;} 
                agentDC.SetCard(card);
                agentDC.Place(this.transform, LOCATION);

                break;
            case CardType.ESSENCE:
                obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

                if(essenceDC == null){return;} 
                essenceDC.SetCard(card);
                essenceDC.Place(this.transform, LOCATION);
                break;
            case CardType.EVENT:
                obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return;} 
                eventDC.SetCard(card);
                eventDC.Place(this.transform, LOCATION);
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
            resetColor = border.color;
            border.color = Color.yellow;

            
            if(pointerEventData.pointerDrag == null) { return;}

            CardDisplay display = pointerEventData.pointerDrag.GetComponent<CardDisplay>();

            if (display == null) {return;}


            if (display.onBoard) {return;}

            if(!CanPlayCardOnThisSpace(display.displayCard)) {return;}

            Card card = display.displayCard;

            Debug.Log("Can be played on this space");



        
        }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(isUnlocked)
        {
            border.color = resetColor;
        }
    }

    bool CanPlayCardOnThisSpace(Card card)
    {
        bool turnManagerApproved = turnManager.CanPlayCard(card);

        return turnManagerApproved;
        
    }
}
