using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class BoardSpace : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    public static string LOCATION = "BOARD";

    public RawImage border;

    public Color resetColor;

    

    public GameObject highlight;
    public RawImage selectionIcon;

    [Header("Managers")]
    Hand hand;
    TurnManager turnManager;

    [Header("Display Prefabs")]
    public GameObject agentDisplayPrefab;
    public GameObject essenceDisplayPrefab;
    public GameObject eventDisplayPrefab;

    [Header("Properties")]
    public bool isUnlocked = false;
    public bool hasEvent = false;
    public bool hasAgent = false;

    public bool isTargetable = false;
    public bool isBeingTargeted = false;
    
    public bool shielded = false;

    public EventCardDisplay eventDisplay;
    // public AgentCardDisplay agentDisplay;

    [Header("Event")]
    public EventCard eventCard;

    [Header("Agent")]
    public AgentCard agentCard;
    public AgentIcon agentIcon;

    void Start()
    {
        hand = Hand.Instance;
        turnManager = TurnManager.Instance;
        border.color = Color.white;
    }


    public void OnDrop(PointerEventData eventData)
    {
        // if (isUnlocked && isTargetable && !isBeingTargeted)
        // {
        //     Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

        //     CardDisplay display = eventData.pointerDrag.GetComponent<CardDisplay>();

        //     if (display == null) {return;}

        //     if (display.onBoard) {return;}

        //     Card card = display.displayCard;

        //     if(card.data.cardType == CardType.ESSENCE)
        //     {
        //         EssenceCard essenceCard = (EssenceCard) card;
                
        //         essenceCard.SelectBoardTarget(this, turnManager.GetCurrentPlayer());  
        //     }
            

        //     // display.inPlaceAnimation = true;
        //     // PlaceCard(display);
        // }
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

        CardType cardType = card.data.cardType;

        GameObject obj = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                return;
            case CardType.ESSENCE:
                return;
            case CardType.EVENT:
                obj = Instantiate(eventDisplayPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return;} 
                EventCard eventCard = (EventCard) card;
                eventDC.SetCard(eventCard);
                eventDC.Place(this.transform, LOCATION);
                SetEventCard(eventDC);
                break;
            default:
            //Error handling
                Debug.LogError ("Invalid CardData Type: " + cardType);
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
        // if(isUnlocked)
        // {
        //     // Highlight();
            
        //     if(pointerEventData.pointerDrag == null) { return;}

        //     CardDisplay display = pointerEventData.pointerDrag.GetComponent<CardDisplay>();

        //     if (display == null) {return;}


        //     if (display.onBoard) {return;}

        //     if(!CanPlayCardOnThisSpace(display.displayCard)) {return;}

        //     turnManager.SetVictoryPointUI();

        // }
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        // if(isUnlocked)
        // {
        //     // EndHighlight();
        // }
    }

    bool CanPlayCardOnThisSpace(Card card)
    {
        bool turnManagerApproved = turnManager.HasEssenceToPlayCard(card);

        return turnManagerApproved;
        
    }

    public void SetEventCard(EventCardDisplay display)
    {
        this.eventDisplay = display;
        this.eventCard = (EventCard) eventDisplay.displayCard;
        this.hasEvent = true;
    }

    public void RemoveEventCard()
    {
        Destroy(this.eventDisplay.gameObject);

        this.eventDisplay = null;
        this.eventCard = null;
        this.hasEvent = false;
    }

    public void SetAgentCard(AgentCard card)
    {
        this.agentCard = card;
        this.hasAgent = true;

        agentIcon.SetAgent(agentCard);
    }

    public void RemoveAgentCard()
    {
        agentIcon.transform.SetAsFirstSibling();

        this.agentCard = null;
        this.hasAgent = false;
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

    public void SelectAsTarget(Texture tex)
    {
        // Debug.Log(tex);
        if(tex == null)
        {
            //Debug.Log("Texture is null, not selecting");
            return;
        }
        selectionIcon.texture = tex;
        selectionIcon.transform.SetAsLastSibling();
        selectionIcon.gameObject.SetActive(true);
        isBeingTargeted = true;
    }

    public void DeselectAsTarget()
    {
        selectionIcon.texture = null;
        selectionIcon.gameObject.SetActive(false);
        isBeingTargeted = false;
    }

    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {

        switch (hand.handState)
        {
            case HandState.CHOOSING:
                //TODO: implement
                break;
            case HandState.TARGET_SELECTION:
                if(isTargetable)
                {
                    hand.SelectBoardTarget(this);
                }
                break;
            default:
                return;
        }

    }

    public void ResolveStartOfTurn()
    {
        if(hasEvent)
        {
            eventDisplay.ResolveStartOfTurn();
        }

        if(hasAgent)
        {
            agentIcon.ResolveStartOfTurn();
        }
    }

    public void AgentEquiptShield(Shield shield)
    {
        agentCard.EquipShield(shield);
        agentIcon.EquipShield();
    }

    public void AgentShieldExpired()
    {
        agentCard.ShieldExpired();
        agentIcon.ShieldExpired();
    }
    
}
