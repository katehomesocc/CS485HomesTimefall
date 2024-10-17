using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Card
{
    public int id;
    public string cardName;
    public string description;

    public Card ()
    {

    }

    public Card(int _id, string _name, string _description)
    {
        id = _id;
        cardName = _name;
        description = _description;
    }
}
