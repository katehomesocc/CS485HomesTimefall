using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New CardData", menuName = "CardData")]
[System.Serializable]
public class CardData : ScriptableObject
{
    
    public int id;
    public string cardName;
    public string description;

    public Texture image;
    public Faction faction;

    public CardType cardType;

    public override string ToString()
    {
        return string.Format("id:{0}, cardName:{1}, desc:{2}, faction:{3}, cardType:{4}", id, cardName, description, faction, cardType);
    }

}
