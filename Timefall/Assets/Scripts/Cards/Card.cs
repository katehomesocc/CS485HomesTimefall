using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public CardData data;
    public List<BoardSpace> targets = new List<BoardSpace>();

    public Card()
    {

    }

    public Card(CardData cardData)
    {
        this.data = cardData;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public virtual void SelectTarget(BoardSpace boardSpace)
    {
        return; //override
    }

    public virtual bool CanBePlayed()
    {
        return false; //override
    }
}
