using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    
    public static List<Card> cardList = new List<Card>();

    void Awake() 
    {
        cardList.Add(new Card(0, "title", "desc"));    
        cardList.Add(new Card(1, "title1", "desc1"));   
        cardList.Add(new Card(2, "title2", "desc2"));   
        cardList.Add(new Card(3, "title3", "desc3"));   
        cardList.Add(new Card(4, "title4", "desc4"));   
    }
}
