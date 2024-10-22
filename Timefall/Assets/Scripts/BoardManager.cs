using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour, IDropHandler
{
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

        PlaceCard(display);
    }

    void PlaceCard(CardDisplay cardDisplay) 
    {
        cardDisplay.Place(this.transform);

        //TODO: physically place card UI
    }
}
