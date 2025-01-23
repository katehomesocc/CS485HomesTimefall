using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    BattleStateMachine battleStateMachine;
    BoardManager boardManager;
    Hand hand;

    int turn = 1;

    public Deck timelineDeck;
    public DiscardPileDisplay inventory;
    public DiscardPileManager discardPileManager;

    public ExpandDisplay expandDisplay;

    public DiceRoller diceRoller;

    [Header("Autoplay (Development Testing)")]
    public bool autoplay = false;
    public int autoplayUntilTurn = 32;
    public float autoplayWaitTime = 1.5f;

    [Header("Start Of Game")]
    public GameObject playerSelectionPanel;
    public int startupTime = 5;
    public GameObject startOfGamePanel;
    public TMP_Text startOfGameText;

    public TMP_Text startOfGameCountdownText;

    [Header("Players")]
    public Player seekerPlayer;
    public Player sovereignPlayer;
    public Player stewardPlayer;
    public Player weaverPlayer;

    [Header("Skyboxes")]
    public Material seekerSkybox;
    public Material sovereignSkybox;
    public Material stewardSkybox;
    public Material weaverSkybox;

    [Header("Card Display Prefabs")]
    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;

    [Header("Colors")]
    public static Color COLOUR_SEEKERS = new Color(33f/255,197f/255,104f/255, 1f);
    public static Color COLOUR_SOVEREIGNS = new Color(255f/255,35f/255,147f/255, 1f);
    public static Color COLOUR_STEWARDS = new Color(24f/255,147f/255,248f/255, 1f);
    public static Color COLOUR_WEAVERS = new Color(97f/255,65f/255,172f/255, 1f);

    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public bool isSettingsOpen = false;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectsSlider;
    public AudioClip UI_EFFECT_SOUND;

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

        masterSlider.onValueChanged.AddListener(delegate {
            UpdateMasterVolume(masterSlider.value);
            PlaySliderEffect();
        });
        
        musicSlider.onValueChanged.AddListener(delegate {
            UpdateMusicVolume(musicSlider.value);
            PlaySliderEffect();
        });
        
        effectsSlider.onValueChanged.AddListener(delegate {
            UpdateEffectVolume(effectsSlider.value);
            PlaySliderEffect();
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        battleStateMachine = BattleStateMachine.Instance;
        boardManager = BoardManager.Instance;
        hand = Hand.Instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            ShowDiscardPiles();
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            HideDiscardPiles();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsMenu();
        }
    }

    public void PlayInitialTimelineCard(CardDisplay display)
    {
        expandDisplay.PlayInitialTimelineCard(display);
    }

    IEnumerator StartOfGame()
    {
        startOfGamePanel.SetActive(true);

        for (int i = startupTime; i > 0; i--)
        {
            startOfGameCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
            
        }

        playerSelectionPanel.SetActive(false);

        startOfGameText.text = "";

        startOfGameCountdownText.text = "BATTLE";
        
        yield return new WaitForSeconds(1);

        startOfGamePanel.SetActive(false);

        battleStateMachine.StartGame();
        
    }

    public ActionRequest GetPotentialTargets(Card card, ActionRequest request)
    {
        if(request.doBoard)
        {
            request.potentialBoardTargets = boardManager.GetPossibleTargets(card, request);
        }

        if(request.doHand)
        {
            request.potentialHandTargets = hand.GetPossibleTargets(card, request);
        }

        if(request.doDiscard)
        {
            request.potentialDiscardedTargets = discardPileManager.GetPossibleTargets(card, request);
        }
           
        return request;
    }

    public void SetPossibleTargetHighlights(Card card, ActionRequest actionRequest)
    {
        if(actionRequest.doBoard)
        {
            boardManager.SetPossibleTargetHighlight(card, actionRequest);
        }

        if(actionRequest.doHand)
        {
            hand.SetPossibleTargetHighlight(card, actionRequest);
        }

        if(actionRequest.doDiscard)
        {
            discardPileManager.SetPossibleTargets(card, actionRequest);
        }
        
        
    }

    public void ClearPossibleTargetHighlights(ActionRequest actionRequest)
    {
        if(actionRequest != null)
        {
            actionRequest.ClearPossibleTargets();
        }
        boardManager.ClearPossibleTargetHighlights();
        hand.ClearPossibleTargetHighlights();
        discardPileManager.ClearAndClosePossibleTargets();
    }

    public Player GetFactionPlayer(Faction faction)
    {
        switch (faction)
        {
            case Faction.STEWARDS:
                return stewardPlayer;
            case Faction.SEEKERS:
                return seekerPlayer;
            case Faction.SOVEREIGNS:
                return sovereignPlayer;   
            case Faction.WEAVERS:
                return weaverPlayer;    
        }

        return null;
    }

    Material GetFactionSkybox(Faction faction)
    {
        switch (faction)
        {
            case Faction.STEWARDS:
                return stewardSkybox;
            case Faction.SEEKERS:
                return seekerSkybox;
            case Faction.SOVEREIGNS:
                return sovereignSkybox; 
            case Faction.WEAVERS:
                return weaverSkybox;  
        }

        return null;
    }

    public static Color GetFactionColor(Faction faction)
    {
        switch(faction) 
        {
            case Faction.WEAVERS:
                return COLOUR_WEAVERS;
            case Faction.SEEKERS:
                return COLOUR_SEEKERS;
            case Faction.SOVEREIGNS:
                return COLOUR_SOVEREIGNS;
            case Faction.STEWARDS:
                return COLOUR_STEWARDS;
        }

        return Color.black;
    }

    public void DiscardToDeck(Card card, Faction faction)
    {
        if(faction == Faction.NONE)
        {
            timelineDeck.Discard(card);
            return;
        }

        Player player = GetFactionPlayer(faction);
        player.deck.Discard(card);
        
    }

    public void SetSkybox(Faction faction)
    {
        RenderSettings.skybox = GetFactionSkybox(faction);
    }

    public void SetAndShowInventory(List<Card> cardsToDisplay)
    {
        inventory.SetInventory(cardsToDisplay);
        inventory.OpenInventory();
    }

    public void ClearAndCloseInventory()
    {
        inventory.ClearInventory();
        inventory.CloseInventory();
    }

    void ShowDiscardPiles()
    {
        discardPileManager.ShowDisplays();
    }

    void HideDiscardPiles()
    {
        discardPileManager.HideDisplays();
    }

    public void ConvertAgent(AgentCard agentToConvert, Faction newFaction)
    {
        Faction oldFaction = agentToConvert.GetFaction();
        Player oldOwner = GetFactionPlayer(oldFaction);
        
        oldOwner.deck.discardPile.Remove(agentToConvert);

        Player newOwner = GetFactionPlayer(newFaction);
        newOwner.ConvertAgent(agentToConvert);
    }

    public CardDisplay ExpandCardView(Card card, bool hoverClear)
    {
        return expandDisplay.ExpandCardView(card, hoverClear);
    }

    public void CloseExpandCardView()
    {
        expandDisplay.CloseExpandCardView();
    }

    public Player GetCurrentPlayer()
    {
        return battleStateMachine.GetCurrentPlayer();
    }

    public void ToggleSettingsMenu()
    {
        if(isSettingsOpen)
        {
            AudioManager.Instance.PlayUIExitMenu();
            settingsPanel.SetActive(false);
            isSettingsOpen = false;
        } else {
            AudioManager.Instance.PlayUIOpenMenu();
            settingsPanel.SetActive(true);
            isSettingsOpen = true;
        }
        
    }

    public void ExitToTitle()
    {
        GameManager.Instance.LoadTitleScene();
    }

    void PlaySliderEffect()
    {
        AudioManager.Instance.PlayUISliderUpdate();
    }

    public void UpdateMasterVolume(float volume)
	{
		AudioManager.Instance.UpdateMasterVolume(volume);
	}

	public void UpdateEffectVolume(float volume)
	{
		AudioManager.Instance.UpdateEffectVolume(volume);
	}

	public void UpdateMusicVolume(float volume)
	{
		AudioManager.Instance.UpdateMusicVolume(volume);
	}
    public static int GetPlayerNumber(Faction faction)
    {
         //0: Stewards, 1: Seekers, 2: Sovereigns, 3: Weavers
        switch(faction)
        {
            case Faction.STEWARDS:
                return 0;
            case Faction.SEEKERS:
                return 1;
            case Faction.SOVEREIGNS:
                return 2;
            case Faction.WEAVERS:
                return 3;
        }

        return 0;
    }

    public static Faction GetFactionFromString(string faction)
    {
        return (Faction) System.Enum.Parse( typeof(Faction), faction );
    }

    public static Faction GetFactionFromIndex(int faction)
    {
        switch(faction)
        {
            case 0:
                return Faction.STEWARDS;
            case 1:
                return Faction.SEEKERS;
            case 2:
                return Faction.SOVEREIGNS;
            case 3:
                return Faction.WEAVERS;
        }

        return Faction.STEWARDS;
    }

    public void SelectPlayersAndStart(PlayerOptions[] playerOptions)
    {
        foreach (PlayerOptions options in playerOptions)
        {
            Player player = GetFactionPlayer(options.selectedFaction);

            player.playerName = options.nameInput.text;
            player.isBot = options.botToggle.isOn;
        }

        StartCoroutine(StartOfGame());
    }

    
}
