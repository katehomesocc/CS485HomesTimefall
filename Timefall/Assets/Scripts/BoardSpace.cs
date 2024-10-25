using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardSpace : MonoBehaviour, IDropHandler
{
    public static string LOCATION = "BOARD";
    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;

    public Card testCard;
    // Start is called before the first frame update
    void Start()
    {
        //SetCard(testCard);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

        CardDisplay display = eventData.pointerDrag.GetComponent<CardDisplay>();

        if (display == null) {return;}

        if (display.onBoard) {return;}

        PlaceCard(display);
    }

    void PlaceCard(CardDisplay cardDisplay) 
    {
        //cardDisplay.Place(this.transform, LOCATION);

        SetCard(cardDisplay.displayCard);
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
}
