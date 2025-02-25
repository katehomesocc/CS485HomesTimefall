using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public CardData data;

    [Header("Effects")]
    public bool channeling = false;
    public bool shielded = false;

    public Shield shield;

    public Card()
    {

    }

    public Card(CardData cardData)
    {
        this.data = cardData;
    }

    public CardType GetCardType()
    {
        return data.cardType;
    }

    public Faction GetFaction()
    {
        return data.faction;
    }

    public Texture GetImageTexture()
    {
        return data.image;
    }

    public string GetCardName()
    {
        return data.cardName;
    }

    public virtual void SelectBoardTarget(ActionRequest actionRequest)
    {
        return; //override
    }

    public virtual void SelectHandTarget(ActionRequest actionRequest)
    {
        return; //override
    }

    public virtual void SelectDiscardTarget(ActionRequest actionRequest)
    {
        return; //override
    }

    public virtual bool CanBePlayed(ActionRequest potentialTargetsRequest)
    {
        return false; //override
    }

    public void StartChannel()
    {
        channeling = true;
    }

        public void EndChannel()
    {
        channeling = false;
    }

    public bool ResolveChannelEffect()
    {
        if(!channeling)
        {
            return false;
        }

        EndChannel();

        return true;
    }

    public virtual void EquipShield(Shield equip)
    {
        shield = equip;
        shielded = true;
    }

    public void ShieldExpired()
    {
        shield = null;
        shielded = false;
    }

    public void ResolveShieldEffect()
    {
        if(!shielded)
        {
            return;
        }

        shield.StartOfTurn();
    }
}
