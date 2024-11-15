using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentIcon : MonoBehaviour
{

    public RawImage border;
    public RawImage agentImage;
    public GameObject shieldIcon;

    public void SetAgent(AgentCard agentCard)
    {
        Debug.Log(string.Format("before set : [{0}]", transform.GetSiblingIndex()));
        border.color = BattleManager.GetFactionColor(agentCard.GetFaction());
        agentImage.texture = agentCard.GetImageTexture();
        this.transform.SetAsLastSibling(); 
        Debug.Log(string.Format("after set : [{0}]", transform.GetSiblingIndex()));
    }

    public void EquipShield()
    {
        shieldIcon.SetActive(true);
    }
    public void ShieldExpired()
    {
        shieldIcon.SetActive(false);
    }
        
}
