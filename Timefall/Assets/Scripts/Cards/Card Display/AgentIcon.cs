using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentIcon : MonoBehaviour
{

    public RawImage border;
    public RawImage agentImage;
    public GameObject shieldIcon;
    
    public AgentCard agentCard;

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
        agentCard.ResolveShieldEffect();
    }
        
}
