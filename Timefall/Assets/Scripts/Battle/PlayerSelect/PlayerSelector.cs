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
            ShowStartInfo();
        }
        

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

    public static void SavePlayerPrefs(PlayerOptions options)
    {
        int playerNumber = options.playerNumber;

        PlayerPrefs.SetString($"playerName{playerNumber}", options.nameInput.text);
        PlayerPrefs.SetString($"playerSelectedFaction{playerNumber}", options.selectedFaction.ToString());
        PlayerPrefs.SetInt($"playerIsBot{playerNumber}", options.botToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt($"playerLocked{playerNumber}", options.Locked ? 1 : 0);
        PlayerPrefs.SetInt($"playerBotDifficulty{playerNumber}", options.difficultyLevel);
    }
    
    public static void LoadPlayerPrefs(PlayerOptions options)
    {
        int playerNumber = options.playerNumber;

        options.nameInput.text = PlayerPrefs.GetString($"playerName{playerNumber}", "");

        options.botToggle.isOn =  PlayerPrefs.GetInt($"playerIsBot{playerNumber}") == 1;
        options.difficultyLevel = PlayerPrefs.GetInt($"playerIsBot{playerNumber}", 0);
        switch(options.difficultyLevel)
        {
            case 1:
                options.easyToggle.isOn = true;
                break;
            case 2:
                options.mediumToggle.isOn = true;
                break;
            case 3:
                options.hardToggle.isOn = true;
                break;
            default:
                options.mediumToggle.isOn = true;
                break;
        }

        string factionString = PlayerPrefs.GetString($"playerSelectedFaction{playerNumber}", "");

        if(string.IsNullOrEmpty(factionString))
        {
            options.selectedFaction = Faction.STEWARDS;
        }
        else
        {
            options.selectedFaction = BattleManager.GetFactionFromString(factionString);
        }

        bool locked = PlayerPrefs.GetInt($"playerLocked{playerNumber}") == 1;
        if(locked)
        {
            options.ToggleLockInButton();
        }
        
        
    }

    public bool SaveAllPlayerPrefs()
    {
        foreach (PlayerOptions options in playerOptions)
        {
            SavePlayerPrefs(options);
        }
        return true;
    }

    void OnApplicationQuit()
    {
        Debug.Log("Saving all player prefs");
        SaveAllPlayerPrefs();
    }
}