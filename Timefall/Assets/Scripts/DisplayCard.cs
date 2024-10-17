using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayCard : MonoBehaviour
{
    public Card displayCard;
    public int displayId;

    public int id;
    public string cardName;
    public string cardDesc;

    public TMP_Text nameText;
    public TMP_Text descText;

    // Start is called before the first frame update
    void Start()
    {
        displayCard = CardDatabase.cardList[displayId];

        id = displayCard.id;
        cardName = displayCard.cardName;
        cardDesc = displayCard.description;     

        nameText.text = " " + cardName;
        descText.text = " " + cardDesc; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
