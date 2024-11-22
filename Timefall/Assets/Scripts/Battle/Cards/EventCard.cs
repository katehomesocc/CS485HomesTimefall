using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCard : Card
{
    public EventCardData eventCardData;

    public EventCard (EventCardData cardData)
    {
        this.data = cardData;
        this.eventCardData = cardData;
    }

    private void Awake() {
        eventCardData = (EventCardData) data;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
