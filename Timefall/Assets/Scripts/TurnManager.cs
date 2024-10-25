using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TurnManager : MonoBehaviour
{
    static int PLAY_ESSENCE_CARD, ROLL_AGENT_EFFECT = 1;
    static int PLAY_AGENT_CARD, DISCARD_HAND_AND_DRAW = 2;
    static int REPLACE_TIMELINE_EVENT = 3;
    int currentTurn;

    public Player seekerPlayer;
    public Player sovereignPlayer;
    public Player stewardPlayer;
    public Player weaverPlayer;
 
    [SerializeField]
    Player currentPlayer;
    [SerializeField]
    private Faction currentFaction; 

    int essenceCount;
    public TMP_Text essenceText;
    public TMP_Text currentTurnText;

    public Button endTurnButton;
    

    // Start is called before the first frame update
    void Start()
    {
        currentTurn = 0;
        essenceCount = 3;

        //any pre game graphics/text

        StartNextFactionTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void EndTurn(){
        EndOfTurnCycle();
    }

    public bool PlayEssenceCard(EssenceCard essenceCard)
    {
        if(essenceCount < PLAY_ESSENCE_CARD) {return false;}

        //do stuff

        return true;
    }

    public bool UseAgentEffect(AgentCard agentCard)
    {
        if(essenceCount < ROLL_AGENT_EFFECT) {return false;}

        //do stuff

        return true;
    }

    public bool PlayAgentCard(AgentCard agentCard)
    {
        if(essenceCount < PLAY_AGENT_CARD) {return false;}

        //do stuff

        return true;
    }

    public bool DiscardHandAndDraw()
    {
        if(essenceCount < DISCARD_HAND_AND_DRAW) {return false;}

        //do stuff

        return true;
    }

    public bool ReplaceTimelineEvent(EventCard timelineEvent, EventCard replaceEvent)
    {
        if(essenceCount < REPLACE_TIMELINE_EVENT) {return false;}

        //do stuff

        return true;
    }

    public void AttemptToSpendTest()
    {
        SpendEssence(1);
    }

    public bool SpendEssence(int cost)
    {
        if(essenceCount < cost) {return false;}

        essenceCount -= cost;
        essenceText.text = string.Format("{0}§", essenceCount);

        //other stuff
        if(essenceCount == 0)
        {
            //set end turn button UI to highlighted as there are no further actions to be taken   
            endTurnButton.GetComponent<Image>().color = Color.yellow;
        }

        return true;
    }

    void EndOfTurnCycle()
    {
        //calculate VP on sectioon played this turn
            //whoever wins gets wins this section of the fabric of time
            //if a tie occurs, Weavers hand size increased
        //All players shuffle deck

        StartNextFactionTurn();
    }

    void StartNextFactionTurn()
    {
        currentTurn++;

        currentPlayer = GetPlayerForThisTurn();
        currentFaction = currentPlayer.faction;

        currentTurnText.text = string.Format("{0} | {1}", currentTurn, currentFaction);

        if(essenceCount == 0)
        {
            //set end turn button UI to highlighted as there are no further actions to be taken   
            endTurnButton.GetComponent<Image>().color = Color.white;
        }

        essenceCount = 3;
        essenceText.text = string.Format("{0}§", essenceCount);

        StartFactionTurn();
    }

    void StartFactionTurn()
    {

        //draw from timeline deck and place on next available spot

        //spend essence

    }

    Player GetPlayerForThisTurn()
    {
        int mod = currentTurn % 4;

        switch(mod)
        {
            case 1:
                return stewardPlayer;
            case 2:
                return seekerPlayer;
            case 3:
                return sovereignPlayer;
            case 0:
                return weaverPlayer;    
        }

        return null;
    }


    
}
