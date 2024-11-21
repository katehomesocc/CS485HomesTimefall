using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    public Card displayCard;
    Hand hand;
    BattleManager battleManager;
    
    [Header("Return Info")]
    public Transform returnParent = null;
    int returnSiblingIndex = 0;
    public int positionInHand = -1;

    [Header("UI")]
    public RawImage selectionImage;
    public TMP_Text nameText;
    public TMP_Text descText;
    public RawImage image;

    [Header("State Info")]
    public CardPlayState playState = CardPlayState.IDK;
    public bool inHand = false;
    public bool onBoard = false;
    public bool onInventory = false;
    public bool isExpanded = false;
    public bool inPlaceAnimation = false;
    public bool isTargetable = false;
    public bool isBeingTargeted = false;

    [Header("Effects")]
    public GameObject channelEffect;

    [Header("Targets")]
    public ActionRequest actionRequest = new ActionRequest();

    // Start is called before the first frame update
    void Start()
    {
        hand = Hand.Instance;
        battleManager = BattleManager.Instance;
        if(displayCard != null)
        {
            nameText.text = displayCard.data.cardName;
            descText.text = displayCard.data.description;
            image.texture = displayCard.data.image;
        }

        HighlightOff();
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!inHand){ return;}

        returnParent = this.transform.parent;
        returnSiblingIndex = this.transform.GetSiblingIndex();

        this.transform.SetParent(this.transform.parent.parent); //edit layer later

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        hand.BeginDragCard(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(string.Format("OnDrag | inHand = {0} | onBoard = {1} ", inHand, onBoard));

        if(!inHand){ return;}

        if(isExpanded)
        {
            battleManager.CloseExpandCardView();
            isExpanded = false;
        }

        this.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        hand.EndDragCard();
        if(inPlaceAnimation){ return;}

        this.transform.SetParent(returnParent, false);   
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
                    break;
                case "BOARD":
                    onBoard = true;
                    break;
                case "INVENTORY":
                    onInventory = true;
                    break;
            }

            hand = Hand.Instance;
            battleManager = BattleManager.Instance;
            //Debug.Log (string.Format("Placing At: {0} | inHand = {1} | onBoard = {2} ", location, inHand, onBoard));
        }
    }

    public void InstantiateInHand(Transform handParent)
    {
        transform.SetParent(handParent, false);

        transform.localScale =  new Vector3(0.55f, 0.55f, 0.55f);

        Place(handParent, "HAND");
    }

    public void InstantiateInInventory(Transform inventoryParent)
    {
        transform.SetParent(inventoryParent, false);

        // transform.localScale =  new Vector3(0.55f, 0.55f, 0.55f);

        Place(inventoryParent, "INVENTORY");
    }

        public void InstantiateInExpand(Transform expandParent)
    {
        transform.SetParent(expandParent, false);

        transform.localScale =  new Vector3(1.5f, 1.5f, 1.5f);

        Place(expandParent, "EXPAND");
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if(!inHand){ return;}

        HighlightOn();

        battleManager.ExpandCardView(displayCard, true);
        isExpanded = true;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if(!inHand){ return;}
        
        HighlightOff();

        battleManager.CloseExpandCardView();
        isExpanded = false;
    }

    public IEnumerator MoveToPosition(Vector3 position, float timeToMove)
    {
        var currentPos = this.transform.position;
        var t = 0f;
        while(t <= 1f)
        {
            t += Time.deltaTime / timeToMove;
            this.transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        this.transform.position = position;
    }

    public IEnumerator ScaleToSize(Vector3 localScale, float timeToMove)
    {
        var currentScale = this.transform.localScale;
        var t = 0f;
        while(t <= 1f)
        {
            t += Time.deltaTime / timeToMove;
            this.transform.localScale = Vector3.Lerp(currentScale, localScale, t);
            yield return null;
        }
        this.transform.localScale = localScale;
    }

    public IEnumerator ScaleToPositionAndSize(Vector3 position, Vector3 localScale, float timeToMove, Transform boardSpaceTransform)
    {
        var currentPos = this.transform.position;
        var currentScale = this.transform.localScale;
        var t = 0f;
        while(t <= 1f)
        {
            t += Time.deltaTime / timeToMove;
            this.transform.position = Vector3.Lerp(currentPos, position, t);
            this.transform.localScale = Vector3.Lerp(currentScale, localScale, t);
            yield return null;
        }
        this.transform.position = position;
        this.transform.localScale = localScale;

        this.transform.SetParent(boardSpaceTransform, false);

        this.transform.localPosition = Vector3.one;
        this.transform.localScale = Vector3.one;
    }

    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {

        switch (hand.handState)
        {
            case HandState.START_TURN_DRAW_TIMELINE:
                if(playState == CardPlayState.START_TURN_DRAW_TIMELINE)
                {
                    battleManager.PlayInitialTimelineCard(this);
                }
                break;
            case HandState.CHOOSING:
                if(inHand)
                {
                    int clickCount = pointerEventData.clickCount;
                    Debug.Log("clickCount = " + clickCount);
                    if(clickCount == 2)
                    {
                        DoubleClickToPlayCard();
                    }
                }
                break;
            case HandState.TARGET_SELECTION:
                if(inHand && isTargetable)
                {
                    hand.SelectHandTarget(this);
                } else if(onInventory && isTargetable)
                {
                    hand.SelectDiscardTarget(this.displayCard);
                }
                break;
            default:    
                return;
        }

    }

    void DoubleClickToPlayCard()
    {
        CardType cardType = GetCardType();

        switch (cardType)
        {
            case CardType.ESSENCE: case CardType.AGENT:
                hand.PlayCard(this);
                return;
            default:    
                return;
        }
    }

    public void SetTargetable(bool targetable)
    {
        isTargetable = targetable;
    }

    public CardType GetCardType()
    {
        return this.displayCard.data.cardType;
    }
    
    public void SelectAsTarget(Texture tex)
    {
        // // Debug.Log(tex);
        // if(tex == null)
        // {
        //     //Debug.Log("Texture is null, not selecting");
        //     return;
        // }
        // selectionIcon.texture = tex;
        // selectionIcon.transform.SetAsLastSibling();
        // selectionIcon.gameObject.SetActive(true);
        isBeingTargeted = true;
    }

    public void DeselectAsTarget()
    {
        // selectionIcon.texture = null;
        // selectionIcon.gameObject.SetActive(false);
        isBeingTargeted = false;
    }

    public void HighlightOn()
    {
        selectionImage.enabled = true;
    }

    public void HighlightOff()
    {
        selectionImage.enabled = false;
    }

    public void ApplyChannelEffect()
    {
        displayCard.StartChannel();
        channelEffect.SetActive(true);
    }

    public void RemoveChannelEffect()
    {
        channelEffect.SetActive(false);
    }

    public int GetPositionInParent()
    {

        return this.transform.GetSiblingIndex();
    }

    public void ResolveStartOfTurn()
    {
        bool resolveChannelEffect = displayCard.ResolveChannelEffect();

        if(resolveChannelEffect)
        {
            RemoveChannelEffect();
        }

        displayCard.ResolveShieldEffect();

        //TODO handle visual effects
    }

    public bool CanBePlayed(Player player)
    {
        actionRequest.player = player;

        Debug.Log(string.Format("CardDisplay.CanBePlayed before: {0}", actionRequest.ToString()));

        actionRequest = BattleManager.Instance.GetPotentialTargets(displayCard, actionRequest);

        Debug.Log(string.Format("CardDisplay.CanBePlayed after: {0}", actionRequest.ToString()));

        return displayCard.CanBePlayed(actionRequest);
    }

}
