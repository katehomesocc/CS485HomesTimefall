using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDisplay : MonoBehaviour, IPointerClickHandler 
{
    public Player player;
    BattleManager battleManager;

    // Start is called before the first frame update
    void Start()
    {
        battleManager = BattleManager.Instance;
    }

    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(pointerEventData.clickCount == 2)
        {
            DoubleClickToViewDiscardPile();
        }
    }

    public void DoubleClickToViewDiscardPile()
    {
        battleManager.SetAndShowInventory(player.deck.discardPile);
    }
}
