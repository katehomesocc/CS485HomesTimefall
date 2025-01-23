using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSelector : MonoBehaviour
{
    public static PlayerSelector Instance = null;
    public List<Faction> availableFactions = new List<Faction>();

    public PlayerOptions[] playerOptions = new PlayerOptions[4];

    public Button startButton;

    [Header("Faction Indicators")]
    public PlayerSelectionIndicator stewardIndicator;
    public PlayerSelectionIndicator seekerIndicator;
    public PlayerSelectionIndicator sovereignIndicator;
    public PlayerSelectionIndicator weaverIndicator;
    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }

        availableFactions.Add(Faction.STEWARDS);
        availableFactions.Add(Faction.SEEKERS);
        availableFactions.Add(Faction.SOVEREIGNS);
        availableFactions.Add(Faction.WEAVERS);

        startButton.onClick.AddListener(SelectPlayersAndStart);

        startButton.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Faction> GetRemainngFactions()
    {
        return availableFactions;
    }

    public bool LockIn(Faction faction, PlayerOptions lockOption)
    {
        if(!availableFactions.Contains(faction)) {return false;}

        lockOption.LockIn();
        GetIndicator(faction).SelectOptions(lockOption);

        availableFactions.Remove(faction);

        if(!LocksRemaining())
        {
            Debug.Log("No locks remaining");
            ShowStartInfo();
        }
        else {Debug.Log("locks remaining");}

        foreach (PlayerOptions options in playerOptions)
        {
            if(options.Locked) {continue;}
            options.Refresh(availableFactions);
        }

        return true;
    }

    public void Unlock(Faction faction)
    {
        availableFactions.Add(faction);

        GetIndicator(faction).Hide();

        HideStartInfo();

        foreach (PlayerOptions options in playerOptions)
        {
            if(options.Locked) {continue;}
            options.Refresh(availableFactions);
        }

    }

    public bool LocksRemaining()
    {
        foreach (PlayerOptions options in playerOptions)
        {
            if(options.Locked) {continue;}
            return true;
        }

        return false;
    }

    void ShowStartInfo()
    {
        startButton.enabled = true;
    }

    void HideStartInfo()
    {
        startButton.enabled = false;
    }

    void SelectPlayersAndStart()
    {
        BattleManager.Instance.SelectPlayersAndStart(playerOptions);
    }

    PlayerSelectionIndicator GetIndicator(Faction faction)
    {
        switch(faction) 
        {
            case Faction.STEWARDS:
                return stewardIndicator;
            case Faction.SEEKERS:
                return seekerIndicator;
            case Faction.SOVEREIGNS:
                return sovereignIndicator;
            case Faction.WEAVERS:
                return weaverIndicator;
        }

        return stewardIndicator;
    }

    public void UpdateIndicator(PlayerOptions options)
    {
        if(!options.Locked) {return;}

        GetIndicator(options.selectedFaction).SelectOptions(options);
    }
}