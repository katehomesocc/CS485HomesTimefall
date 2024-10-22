using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour, IDropHandler
{
    public static string LOCATION = "BOARD";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

        CardDisplay display = eventData.pointerDrag.GetComponent<CardDisplay>();

        if (display == null) {return;}

        if (display.onBoard) {return;}

        PlaceCard(display);
    }

    void PlaceCard(CardDisplay cardDisplay) 
    {
        cardDisplay.Place(this.transform, LOCATION);

        //TODO: physically place card UI
    }
}
