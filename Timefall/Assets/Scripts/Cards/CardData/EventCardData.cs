using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event CardData", menuName = "CardData/EventCardData")]
public class EventCardData : CardData
{
    public int[] victoryPoints = new int[4]; //0: Stewards, 1: Seekers, 2: Sovereigns, 3: Weavers

    public void Awake()
    {
        cardType = CardType.EVENT;
    }
    
}
