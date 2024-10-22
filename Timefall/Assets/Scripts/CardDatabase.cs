using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card DB", menuName = "CardDB")]
public class CardDatabase : ScriptableObject
{
   
    public List<Card> cardList = new List<Card>();

    void Awake() 
    {   
        cardList.Sort((x, y) => x.id.CompareTo(y.id));
    }
}
