using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSelectionIndicator : MonoBehaviour
{
    public GameObject indicator;
    public GameObject botIcon;
    public TMP_Text text;

    public void Hide()
    {
        indicator.SetActive(false);
    }

    public void SelectOptions(PlayerOptions options)
    {
        if(options.botToggle.isOn)
        {
            SelectBot();
            return;
        }

        SelectPlayer(options.playerNumber);
    }

    void SelectBot()
    {
        botIcon.SetActive(true);
        text.text = "";
        indicator.SetActive(true);
        
    }
    void SelectPlayer(int number)
    {
        botIcon.SetActive(false);
        text.text = $"P{number}";
        indicator.SetActive(true);        
    }
}
