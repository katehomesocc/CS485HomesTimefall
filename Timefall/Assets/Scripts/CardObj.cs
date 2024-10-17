using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardObj : ScriptableObject
{
    public int id;
    public string cardName;
    public string description;

    public Texture image;

    public void Print()
    {
        Debug.Log(cardName + ": " + description);
    }
}
