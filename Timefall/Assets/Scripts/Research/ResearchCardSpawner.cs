using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchCardSpawner : MonoBehaviour
{
    [Header("Card Databases")]

    public ResearchCardDB TimelineDB;
    public ResearchCardDB StewardDB;
    public ResearchCardDB SeekerDB;
    public ResearchCardDB SovereignDB;
    public ResearchCardDB WeaverDB;

    public List<CardData> GetInitial()
    {
        return TimelineDB.GetAll();
    }

}
