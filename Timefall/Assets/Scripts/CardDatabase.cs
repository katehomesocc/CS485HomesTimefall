using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
   
    public List<Card> cardList = new List<Card>();

    void Awake() 
    {   
        cardList.Sort((x, y) => x.id.CompareTo(y.id));
    }
}
