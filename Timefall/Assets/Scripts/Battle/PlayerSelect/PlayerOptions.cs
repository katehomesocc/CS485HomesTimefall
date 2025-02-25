using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerOptions : MonoBehaviour
{
    [Header("Lock In")]
    public Button lockInButton;
    public bool Locked { get; private set;}

    [Header("Player Name")]
    public int playerNumber = 1;
    public TMP_InputField nameInput;

    [Header("Bot Settings")]
    public Toggle botToggle;
    public GameObject difficultyCover;
    public Toggle easyToggle;
    public Toggle mediumToggle;
    public Toggle hardToggle;
    public int difficultyLevel = 2;

    [Header("Faction Selection")]
    public Faction selectedFaction = Faction.STEWARDS;
    public TMP_Dropdown factionsDropdown;
    public Button buttonLeft;
    public Button buttonRight;
    public List<Faction> availableFactions = new List<Faction>();
    private int listPos = 0;

    void Awake()
    {
        listPos = BattleManager.GetPlayerNumber(selectedFaction);

        buttonLeft.onClick.AddListener(ListDown);
        buttonRight.onClick.AddListener(ListUp);
        lockInButton.onClick.AddListener(ToggleLockInButton);

        botToggle.onValueChanged.AddListener(delegate { ToggleBot(botToggle.isOn); });

        easyToggle.onValueChanged.AddListener(delegate {
                ToggleDifficulty(easyToggle, 1);
            });
        mediumToggle.onValueChanged.AddListener(delegate {
                ToggleDifficulty(mediumToggle, 2);
        });
        hardToggle.onValueChanged.AddListener(delegate {
                ToggleDifficulty(hardToggle, 3);
            });
    }

    // Start is called before the first frame update
    void Start()
    {
        availableFactions = PlayerSelector.Instance.GetRemainngFactions();
        Refresh(availableFactions);

        PlayerSelector.LoadPlayerPrefs(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ListUp()
    {
        listPos++;

        buttonLeft.interactable = true;

        SelectFaction(listPos);
    }

    public void ListDown()
    {
        listPos--;

        buttonRight.interactable = true;

        SelectFaction(listPos);
    }

    void SelectFaction(int factionNumber)
    {
        if(factionNumber == 0)
        {
            buttonLeft.interactable = false;
        } else if(factionNumber == 3)
        {
            buttonRight.interactable = false;
        }

        selectedFaction = BattleManager.GetFactionFromIndex(factionNumber);

        Faction previous = selectedFaction;
        Faction next = selectedFaction;

        if(listPos != 0)
        {
            previous = BattleManager.GetFactionFromIndex(factionNumber - 1);
        } 

        if(listPos != 3)
        {
            next = BattleManager.GetFactionFromIndex(factionNumber + 1);
        }

        bool available = IsAvailable(selectedFaction);

        SetFactionColor(selectedFaction, previous, next, available);

        factionsDropdown.value = factionNumber;

        lockInButton.interactable = available;
    }

    bool IsAvailable(Faction faction)
    {
        foreach (Faction availFact in availableFactions)
        {
            if(faction == availFact)
            {
                return true;
            }
        }

        return false;
    }

    void SetFactionColor(Faction faction, Faction previous, Faction next, bool available)
    {
        Color selectedColor = BattleManager.GetFactionColor(faction);

        // buttonLeft.targetGraphic.color =  BattleManager.GetFactionColor(previous);
        
        // buttonRight.targetGraphic.color =  BattleManager.GetFactionColor(next);

        if(available)
        {
            factionsDropdown.targetGraphic.color = selectedColor;
            return;
        }

        factionsDropdown.targetGraphic.color = Color.white;
    }

    public void ToggleLockInButton()
    {
        if(Locked)
        {
            Unlock(selectedFaction);
            return;
        }

        AttemptLockIn(selectedFaction);

    }

    void AttemptLockIn(Faction faction)
    {
        PlayerSelector.Instance.LockIn(faction, this);
    }

    public void LockIn()
    {
        Locked = true;

        buttonLeft.interactable = false;
        buttonRight.interactable = false;

        lockInButton.GetComponentInChildren<TMP_Text>().text = "Unlock";

        //TODO: change button to say unlock
    }

    void Unlock(Faction faction)
    {
        Locked = false;
        buttonLeft.interactable = true;
        buttonRight.interactable = true;

        lockInButton.GetComponentInChildren<TMP_Text>().text = "Lock In";

        PlayerSelector.Instance.Unlock(faction);
    }

    public void Refresh(List<Faction> factions)
    {
        availableFactions = factions;

        SelectFaction(listPos);
    }

    void ToggleBot(bool isBot)
    {
        difficultyCover.SetActive(!isBot);

        if(!Locked) {return;}

        PlayerSelector.Instance.UpdateIndicator(this);
    }

    void ToggleDifficulty(Toggle toggle, int level)
    {
        RectTransform rect = toggle.GetComponentInChildren<RectTransform>();

        if(!toggle.isOn){
            rect.sizeDelta = new Vector2 (25, 25);
            return;
        }

        rect.sizeDelta = new Vector2 (40, 40);

        difficultyLevel = level;
    }
}
