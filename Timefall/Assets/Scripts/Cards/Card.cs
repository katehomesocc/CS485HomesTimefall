using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public CardData data;
    public List<BoardSpace> boardTargets = new List<BoardSpace>();
    public List<CardDisplay> handTargets = new List<CardDisplay>();

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

    public virtual void SelectTarget(BoardSpace boardSpace, Player player)
    {
        return; //override
    }

    public virtual void SelectTarget(CardDisplay handTarget, Player player)
    {
        return; //override
    }

    public virtual bool CanBePlayed()
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

    public void EquipShield(Shield equip)
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
