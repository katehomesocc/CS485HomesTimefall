using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RC DB", menuName = "CardDB | Research")]
public class ResearchCardDB : ScriptableObject
{
    public string dbName = "New DB";

    public int totalSize = 0;
    List<CardData> cardList = new List<CardData>();
    public List<CardDBEntry> agents = new List<CardDBEntry>();
    public List<CardDBEntry> essence = new List<CardDBEntry>();
    public List<CardDBEntry> events = new List<CardDBEntry>();

    void Awake() 
    {   
        cardList.Sort((x, y) => x.id.CompareTo(y.id));
    }

    void BuildCardList()
    {
        cardList.Clear();
        cardList.AddRange(GetCalulatedEntries(events));
        cardList.AddRange(GetCalulatedEntries(essence));
        cardList.AddRange(GetCalulatedEntries(agents));
    }

    List<CardData> GetCalulatedEntries(List<CardDBEntry> entries)
    {
        List<CardData> returnList = new List<CardData>();

        foreach (CardDBEntry entry in entries)
        {
            for (int i = 1; i <= entry.amount; i++)
            {
                returnList.Add(entry.card);
            }
        }

        return returnList;
    }

    public int CalcTotalSize()
    {
        totalSize = 0;
        totalSize += CalcEntrySize(events);
        totalSize += CalcEntrySize(essence);
        totalSize += CalcEntrySize(agents);
        return totalSize;
    }

    int CalcEntrySize(List<CardDBEntry> entries)
    {
        int total = 0;
        foreach (CardDBEntry entry in entries)
        {
            total += entry.amount;
        }

        return total;
    }

    /*
    * sorting functions
    */

    public List<CardData> GetAll()
    {
        BuildCardList();
        return cardList;
    }

    // public List<CardData> GetAgents()
    // {
    //     return agents;
    // }

    // public List<CardData> GetEvents()
    // {
    //     return events;
    // }

    // public List<CardData> GetEssence()
    // {
    //     return essence;
    // }
}
