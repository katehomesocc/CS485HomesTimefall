using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class BoardSpace : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    public int spaceNumber;
    public RawImage border;

    public Color resetColor;

    public GameObject highlight;
    public RawImage selectionIcon;
    public GameObject eventSpawn;

    [Header("Managers")]
    Hand hand;
    BattleStateMachine battleStateMachine;
    BoardManager boardManager;

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

    public bool isHole = false;

    public EventCardDisplay eventDisplay;

    [Header("Event")]
    public EventCard eventCard;
    public ShieldDisplay eventShield;

    [Header("Agent")]
    public AgentCard agentCard;
    public AgentIcon agentIcon;

    void Start()
    {
        hand = Hand.Instance;
        battleStateMachine = BattleStateMachine.Instance;
        boardManager = BoardManager.Instance;
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
                
        //         essenceCard.SelectBoardTarget(this, battleStateMachine.GetCurrentPlayer());  
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
                eventDC.Place(this.transform, "BOARD");
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

        //     battleStateMachine.SetVictoryPointUI();

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
        bool battleStateMachineApproved = battleStateMachine.HasEssenceToPlayCard(card);

        return battleStateMachineApproved;
        
    }

    public void SetEventCard(EventCardDisplay display)
    {
        this.eventDisplay = display;
        this.eventCard = (EventCard) eventDisplay.displayCard;
        this.hasEvent = true;

        display.Place(this.transform, "BOARD");
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

    public void RemoveAgentCard(bool onDeath)
    {
         Debug.Log(string.Format("[{0}] | RemoveAgentCard", this.name));
        //TODO: animation

        agentIcon.transform.SetAsFirstSibling();

        if(onDeath)
        {
            Debug.Log(string.Format("[{0}] | [{1}] died", this.name, agentCard.data.cardName));
            agentCard.Death();
        }
        
        this.agentCard = null;
        this.hasAgent = false;

        // if(shielded)
        // {
        //     EventShieldExpired();
        // }
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

    public void ResolveEndOfTurn()
    {
        if(hasEvent)
        {
            eventDisplay.ResolveEndOfTurn();
        }

        if(hasAgent)
        {
            agentIcon.ResolveEndOfTurn();
        }
    }

    public void AgentEquiptShield(Shield shield)
    {
        agentCard.EquipShield(shield);
        agentIcon.EquiptShield(shield);
    }

    public void AgentShieldExpired()
    {
        Debug.Log(string.Format("[{0}] |  AgentShieldExpired", this.name));
    
        agentCard.ShieldExpired();
        agentIcon.ShieldExpired();
    }

    public void AgentShieldCountdown()
    {
        agentIcon.DecreaseShieldCountDown();
    }

    public void EventEquiptShield(Shield shield)
    {
        shielded = true;
        eventCard.EquipShield(shield);
        eventShield.SetShield(shield);
    }

    public void EventShieldExpired()
    {
        shielded = false;
        eventCard.ShieldExpired();
        eventShield.Expire();
    }

    public void EventShieldCountdown()
    {
        eventShield.DecreaseCountDown();
    }

    public IEnumerator PlaceEventOn(EventCardDisplay cardDisplay)
    {
        yield return cardDisplay.ScaleToPositionAndSize(eventSpawn.transform.position, eventSpawn.transform.lossyScale, 1f, eventSpawn.transform);
        
        SetEventCard(cardDisplay);
        
        cardDisplay.SetCardPlayState(CardPlayState.ON_BOARD);
    }

    public void CosmicBlast()
    {
        AgentCard agentToDiscard = agentCard;

        //send agent to its factions discard pile
        BattleManager.Instance.DiscardToDeck(agentToDiscard, agentToDiscard.GetFaction());

        RemoveAgentCard(true);


    }

    public void Paradox()
    {
        //send event to timeline discard
        EventCard eventToDiscard = eventCard;
        RemoveEventCard();
        BattleManager.Instance.DiscardToDeck(eventToDiscard, Faction.NONE);
    
        if(hasAgent)
        {
            //send agent to its factions discard pile
            AgentCard agentToDiscard = agentCard;
            RemoveAgentCard(true);
            BattleManager.Instance.DiscardToDeck(agentToDiscard, agentToDiscard.GetFaction());
        }

        isHole = true;
    }

    public void Patch(EventCardDisplay display)
    {
        StartCoroutine(PatchAnimation(display));
    }

    IEnumerator PatchAnimation(EventCardDisplay display)
    {
        //TODO: patch audio effect

        yield return new WaitForSeconds(1f);

        BattleManager.Instance.expandDisplay.RemoveStaticDisplay(display);

        yield return PlaceEventOn(display);

        battleStateMachine.SetVictoryPointUI();

        isHole = false;
    }

    public void Replace(EventCardDisplay replacementDisplay, EssenceAction essenceAction, ActionRequest actionRequest)
    {
        StartCoroutine(ReplaceAnimation(replacementDisplay, essenceAction, actionRequest));
    }

    IEnumerator ReplaceAnimation(EventCardDisplay display, EssenceAction essenceAction, ActionRequest actionRequest)
    {
        //TODO: replace audio effect

        RemoveEventCard();

        yield return new WaitForSeconds(0.25f);

        yield return PlaceEventOn(display);

        battleStateMachine.SetVictoryPointUI();

        EndCallbackAction(essenceAction, actionRequest);
    }

    public void EndCallbackAction(EssenceAction essenceAction, ActionRequest actionRequest)
    {
        Debug.Log("BoardSpace EndCallbackAction ");
        essenceAction.EndAction(actionRequest);
    }

    public override bool Equals(object obj)
    {
        var space = obj as BoardSpace;

        if (space == null)
        {
            return false;
        }

        return this.spaceNumber.Equals(space.spaceNumber);
    }

    public override int GetHashCode()
    {
        return this.spaceNumber.GetHashCode();
    }
}