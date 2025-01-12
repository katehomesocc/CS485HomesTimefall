using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FactionToggle : MonoBehaviour
{
    public Toggle toggle;
    public Faction faction;
    public Text toggleText;

    public void SetTextColor(Color color)
    {
        toggleText.color = color;
    }
}
