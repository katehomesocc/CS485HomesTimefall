using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCard : Card
{
    public AgentCardData agentCardData;

    public AgentCard (AgentCardData cardData)
    {
        this.data = cardData;
        this.agentCardData = cardData;
    }

    private void Awake() {
        agentCardData = (AgentCardData) data;
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
