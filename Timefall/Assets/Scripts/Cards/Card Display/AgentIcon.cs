using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AgentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    Hand hand;
    BattleManager battleManager;
    public RawImage border;
    public RawImage agentImage;
    public GameObject shieldIcon;
    
    public AgentCard agentCard;

    bool isExpanded = false;

    void Start()
    {
        hand = Hand.Instance;
        battleManager = BattleManager.Instance;
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
        Debug.LogError("//TODO: roll for action");
        //TODO: roll for action
    }
        
}
