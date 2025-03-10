using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchCardSpawner : MonoBehaviour
{
    public ResearchScrollableDisplay researchDisplay;
    public HashSet<Faction> activeFactions = new HashSet<Faction>();

    public CardType cardType = CardType.AGENT;

    [Header("Card Databases")]
    public ResearchCardDB TimelineDB;
    public ResearchCardDB StewardDB;
    public ResearchCardDB SeekerDB;
    public ResearchCardDB SovereignDB;
    public ResearchCardDB WeaverDB;

    [Header("Card Type Icon Textures")]
    public Texture AGENT_ICON_TEX;
    public Texture ESSENCE_ICON_TEX;
    public Texture EVENT_ICON_TEX;

    void Awake()
    {
        activeFactions.Add(Faction.STEWARDS);
        activeFactions.Add(Faction.SEEKERS);
        activeFactions.Add(Faction.SOVEREIGNS);
        activeFactions.Add(Faction.WEAVERS);
    }

    public void SpawnInitialLayout()
    {
        Spawn();
    }

    public void Spawn()
    {
        switch (cardType)
            {
                case CardType.AGENT:
                    SpawnAgents();
                    break;
                case CardType.ESSENCE:
                    SpawnEssence();
                    break;
                case CardType.EVENT:
                    SpawnEvents();
                    break;
            }
    }

    public void SelectCardType(CardType type)
    {
        cardType = type;
        Spawn();
    }

    public void AddActiveFaction(Faction faction)
    {
        activeFactions.Add(faction);
        Spawn();
    }

    public void RemoveActiveFaction(Faction faction)
    {
        activeFactions.Remove(faction);
        Spawn();
    }

    void SpawnAgents()
    {
        researchDisplay.SetHeaderIcon(AGENT_ICON_TEX);
        researchDisplay.SetHeaderText("Agents");
        researchDisplay.SetContent(GetAgentCardData());
    }
    
    void SpawnEssence()
    {
        researchDisplay.SetHeaderIcon(ESSENCE_ICON_TEX);
        researchDisplay.SetHeaderText("Essence");
        researchDisplay.SetContent(GetEssenceCardData());
    }

    void SpawnEvents()
    {
        researchDisplay.SetHeaderIcon(EVENT_ICON_TEX);
        researchDisplay.SetHeaderText("Events");
        researchDisplay.SetContent(GetEventCardData());
    }

    List<CardData> GetEssenceCardData()
    {
        //TODO: handle faction #'s
        return TimelineDB.GetEssence();
    }

    List<CardData> GetAgentCardData()
    {
        List<CardData> agentCardData = new List<CardData>();

        if(activeFactions.Contains(Faction.STEWARDS))
        {
            agentCardData.AddRange(StewardDB.GetAgents());
        }

        if(activeFactions.Contains(Faction.SEEKERS))
        {
            agentCardData.AddRange(SeekerDB.GetAgents());
        }

        if(activeFactions.Contains(Faction.SOVEREIGNS))
        {
            agentCardData.AddRange(SovereignDB.GetAgents());
        }

        if(activeFactions.Contains(Faction.WEAVERS))
        {
            agentCardData.AddRange(WeaverDB.GetAgents());
        }
        return agentCardData;
    }

    List<CardData> GetEventCardData()
    {
        List<CardData> eventCardData = new List<CardData>();

        if(activeFactions.Contains(Faction.STEWARDS))
        {
            eventCardData.AddRange(StewardDB.GetEvents());
        }

        if(activeFactions.Contains(Faction.SEEKERS))
        {
            eventCardData.AddRange(SeekerDB.GetEvents());
        }

        if(activeFactions.Contains(Faction.SOVEREIGNS))
        {
            eventCardData.AddRange(SovereignDB.GetEvents());
        }

        if(activeFactions.Contains(Faction.WEAVERS))
        {
            eventCardData.AddRange(WeaverDB.GetEvents());
        }

        eventCardData.AddRange(TimelineDB.GetEvents());
        
        return eventCardData;
    }



}
