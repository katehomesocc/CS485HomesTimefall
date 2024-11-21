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
    public GameObject shieldIcon;
    
    public AgentCard agentCard;

    bool isExpanded = false;

    Gradient gradient;

    public bool isAttemptable = false;

    public float gradStep = 0.0f;

    public float flashSpeed = 1.0f;

    void Start()
    {
        hand = Hand.Instance;
        battleManager = BattleManager.Instance;
        attemptableImage = GetComponent<RawImage>();

        gradient = GetGradient();
    }

    void Update()
    {
        if(!isAttemptable){return;}
        
        HighlightAttemptable();
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
    }

    public void EquipShield()
    {
        shieldIcon.SetActive(true);
    }
    public void ShieldExpired()
    {
        shieldIcon.SetActive(false);
    }

    public void ResolveStartOfTurn()
    {
        agentCard.attempted = false;
        agentCard.ResolveShieldEffect();

        if(agentCard.GetFaction() == battleManager.GetCurrentPlayer().faction)
        {
            isAttemptable = true;
        }
    }

    public void ResolveEndOfTurn()
    {
        if(isAttemptable){
            isAttemptable = false;
            RemoveAttemptHighlight();
        }

        if(agentCard.attempted){
            agentCard.attempted = false;
        }
        
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //HighlightOn();

        battleManager.ExpandCardView(agentCard, true);
        this.isExpanded = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //HighlightOff();

        battleManager.CloseExpandCardView();
        this.isExpanded = false;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        switch (hand.handState)
        {
            case HandState.CHOOSING:
                int clickCount = pointerEventData.clickCount;

                if(clickCount == 2)
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
        Debug.Log("//TODO: roll for action");
        //TODO: roll for action

        isAttemptable = false;
        RemoveAttemptHighlight();
        agentCard.attempted = true;
    }

    void HighlightAttemptable()
    {
        if(gradStep < 1.0f)
        {
            gradStep += 0.001f * flashSpeed;
        } else
        {
            gradStep = 0.0f;
        }
        attemptableImage.color = GetAttemptedColor();
    }

    void RemoveAttemptHighlight()
    {
        Color tempColor = attemptableImage.color;

        Color newColor = new Color(tempColor.r,tempColor.g, tempColor.b, 0f);

        attemptableImage.color = newColor;
    }

    Color GetAttemptedColor()
    {
        return gradient.Evaluate(gradStep);
    }
        
}
