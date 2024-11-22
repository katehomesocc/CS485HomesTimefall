using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CardData DB", menuName = "CardDB")]
public class CardDatabase : ScriptableObject
{
   
    public List<CardData> cardList = new List<CardData>();

    void Awake() 
    {   
        cardList.Sort((x, y) => x.id.CompareTo(y.id));
    }
}
