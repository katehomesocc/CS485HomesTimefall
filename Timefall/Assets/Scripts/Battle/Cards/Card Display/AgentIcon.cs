using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AgentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    Hand hand;
    BattleManager battleManager;
    public RawImage attemptableImage;
    public RawImage border;
    public RawImage agentImage;
    public ShieldDisplay shieldDisplay;
    
    public AgentCard agentCard;

    Gradient gradient;

    public bool isAttemptable = false;
    public bool placedThisTurn = false;

    public float gradStep = 0.0f;

    public float flashSpeed = 1.0f;

    public Material attemptableMat;

    [Header("Targets")]
    public ActionRequest actionRequest = new ActionRequest();

    void Start()
    {
        hand = Hand.Instance;
        battleManager = BattleManager.Instance;
        attemptableImage = GetComponent<RawImage>();

        gradient = GetGradient();
    }

    static Gradient GetGradient()
    {
        Gradient grad = new Gradient();

        var colors = new GradientColorKey[6];
        colors[0] = new GradientColorKey(BattleManager.COLOUR_STEWARDS, 0.0f);
        colors[1] = new GradientColorKey(BattleManager.COLOUR_SEEKERS, 0.25f);
        colors[2] = new GradientColorKey(BattleManager.COLOUR_SOVEREIGNS, 0.50f);
        colors[3] = new GradientColorKey(BattleManager.COLOUR_WEAVERS, 0.75f);
        colors[4] = new GradientColorKey(BattleManager.COLOUR_WEAVERS, 0.90f);
        colors[5] = new GradientColorKey(BattleManager.COLOUR_STEWARDS, 1.0f);

        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        grad.SetKeys(colors, alphas);

        return grad;
    }

    public void SetAgent(AgentCard agent)
    {
        this.agentCard = agent;

        this.border.color = BattleManager.GetFactionColor(agentCard.GetFaction());
        this.agentImage.texture = agentCard.GetImageTexture();

        this.transform.SetAsLastSibling();

        isAttemptable = true;

        placedThisTurn = true;

        agent.isOnBoard = true;

        agentCard.SetActionRequest(actionRequest);
    }

    public void EquiptShield(Shield shield)
    {
        shieldDisplay.SetShield(shield);
    }
    public void ShieldExpired()
    {
        shieldDisplay.Expire();
    }

    public void ResolveStartOfTurn()
    {
        agentCard.attempted = false;
        agentCard.ResolveShieldEffect();

        if(agentCard.GetFaction() == battleManager.GetCurrentPlayer().faction)
        {
            isAttemptable = true;
            HighlightAttemptable();
        }
    }

    public void ResolveEndOfTurn()
    {
        if(isAttemptable){
            isAttemptable = false;
            RemoveAttemptHighlight();
        }
        
        agentCard.attempted = false;
        placedThisTurn = false;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //HighlightOn();

        battleManager.ExpandCardView(agentCard, true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //HighlightOff();

        battleManager.CloseExpandCardView();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        switch (hand.handState)
        {
            case HandState.CHOOSING:
                int clickCount = pointerEventData.clickCount;

                if(clickCount == 2 && isAttemptable)
                {
                    DoubleClickToRollForAction();
                }

                break;
            default:    
                return;
        }
    }

    void DoubleClickToRollForAction()
    {
        if(agentCard.attempted)
        {
            Debug.Log(string.Format("already attempted {0} this turn", agentCard.GetCardName()));
            return;
        }

        isAttemptable = false;
        RemoveAttemptHighlight();

        hand.AttemptAgentAction(this);
    }

    void HighlightAttemptable()
    {
        Color tempColor = attemptableImage.color;

        Color newColor = new Color(tempColor.r,tempColor.g, tempColor.b, 1f);

        attemptableImage.color = newColor;
        attemptableImage.material = attemptableMat;
    }

    void RemoveAttemptHighlight()
    {
        Color tempColor = attemptableImage.color;

        Color newColor = new Color(tempColor.r,tempColor.g, tempColor.b, 0f);

        attemptableImage.color = newColor;
        attemptableImage.material = null;
    }

    public void AttemptAction()
    {
        //TODO: animation

        agentCard.attempted = true;
        agentCard.RollForAction(this);
    }

    public void SuccessCallback()
    {

        Debug.Log("AgentIcon callback ... success!");

        hand.StartAgentAction();

        agentCard.SuccessCallback(actionRequest);
    
    }
    public void FailureCallback()
    {
        Debug.Log("AgentIcon callback ... failure");

        agentCard.FailureCallback();

        hand.EndAgentAction();
    }

    public void DecreaseShieldCountDown()
    {
        shieldDisplay.DecreaseCountDown();
    }
        
}
