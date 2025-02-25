using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAction : MonoBehaviour
{
    public enum Actions
    {
        PlayEssenceCard,
        RollAgentEffect,
        PlayAgentCard,
        DiscardHandAndDraw,
        ReplaceTimelineEvent
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void ReplaceTimelineEvent(CardDisplay eventToPlay, BoardSpace targetSpace)
    {
        eventToPlay.actionRequest.isBot = true;
        eventToPlay.actionRequest.activeHandTargets.Add(eventToPlay);
        Hand.Instance.PlayCard(eventToPlay, true); // Play the event card
    }
}
