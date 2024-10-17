using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayCardObj : MonoBehaviour
{
    public CardObj displayCard;

    public TMP_Text nameText;
    public TMP_Text descText;
    public RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = displayCard.cardName;
        descText.text = displayCard.description;

        image.texture = displayCard.image;
    }

}
