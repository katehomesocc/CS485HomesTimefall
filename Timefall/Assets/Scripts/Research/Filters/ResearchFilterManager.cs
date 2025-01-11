using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchFilterManager : MonoBehaviour
{
    public static Color DEFAULT_FILTER_COLOR = new Color(188f/255,193f/255,187f/255, 1f);
    ResearchManager researchManager;
    public bool singleFactionView = false;
    public RawImage filterBackground;

    [Header("Faction Filters")]
    public Toggle allFactionsToggle;
    public FactionToggle stewardsToggle;
    public FactionToggle seekersToggle;
    public FactionToggle sovereignsToggle;
    public FactionToggle weaversToggle;

    [Header("Card Type Filters")]
    public Toggle allCardsToggle;
    public Toggle agentsToggle;
    public Toggle essenceToggle;
    public Toggle eventsToggle;

    public HashSet<FactionToggle> activeFactions = new HashSet<FactionToggle>();
    public HashSet<Toggle> activeCardTypes = new HashSet<Toggle>();
    
    // Start is called before the first frame update
    void Start()
    {
        researchManager = ResearchManager.Instance;

        activeFactions.Add(stewardsToggle);
        activeFactions.Add(seekersToggle);
        activeFactions.Add(sovereignsToggle);
        activeFactions.Add(weaversToggle);

        activeCardTypes.Add(agentsToggle);
        activeCardTypes.Add(essenceToggle);
        activeCardTypes.Add(eventsToggle);


        allFactionsToggle.onValueChanged.AddListener(delegate {
                ToggleAllFactions();
            });
        stewardsToggle.toggle.onValueChanged.AddListener(delegate {
                ToggleFaction(stewardsToggle);
            });
        seekersToggle.toggle.onValueChanged.AddListener(delegate {
                ToggleFaction(seekersToggle);
            });
        sovereignsToggle.toggle.onValueChanged.AddListener(delegate {
                ToggleFaction(sovereignsToggle);
            });
        weaversToggle.toggle.onValueChanged.AddListener(delegate {
                ToggleFaction(weaversToggle);
            });

        allCardsToggle.onValueChanged.AddListener(delegate {
                ToggleAllCardTypes();
            });
        agentsToggle.onValueChanged.AddListener(delegate {
                ToggleCardType(agentsToggle, CardType.AGENT);
            });
        essenceToggle.onValueChanged.AddListener(delegate {
                ToggleCardType(essenceToggle, CardType.ESSENCE);
            });
        eventsToggle.onValueChanged.AddListener(delegate {
                ToggleCardType(eventsToggle, CardType.EVENT);
            });

        Debug.Log(string.Format("Start: [{0}] active toggles", activeFactions.Count));
    }

    void ToggleAllFactions()
    {
        if(allFactionsToggle.isOn){
            Debug.Log("ToggleAllFactions ON");
            allFactionsToggle.interactable = false;
            stewardsToggle.toggle.isOn = true;
            seekersToggle.toggle.isOn = true;
            sovereignsToggle.toggle.isOn = true;
            weaversToggle.toggle.isOn = true;
        } else 
        {
            Debug.Log("ToggleAllFactions OFF");
            allFactionsToggle.interactable = true;
        }
    }

    void ToggleFaction(FactionToggle factionToggle)
    {
        Toggle toggle = factionToggle.toggle;
        Faction faction =  factionToggle.faction;
        if(toggle.isOn){
            Debug.Log(string.Format("Toggle{0} ON", faction));
            activeFactions.Add(factionToggle);
            if(singleFactionView)
            {
                LeaveSingleFactionView();
            }
        } else 
        {
            if (activeFactions.Count == 2)
            {
                Debug.Log(string.Format("Toggle{0} OFF", faction));
                activeFactions.Remove(factionToggle);
                EnterSingleFactionView();
            }
            else if(activeFactions.Count == 1)
            {
                toggle.isOn = true;
                return;
            }
            else
            {
                Debug.Log(string.Format("Toggle{0} OFF", faction));
                activeFactions.Remove(factionToggle);
                allFactionsToggle.isOn = false;
            }
            
            
        }

        SetFactionToggleColor(toggle, faction);
    }

    void ToggleAllCardTypes()
    {
        if(allCardsToggle.isOn){
            Debug.Log("ToggleAllCardTypes ON");
            allCardsToggle.interactable = false;
            stewardsToggle.toggle.isOn = true;
            seekersToggle.toggle.isOn = true;
            sovereignsToggle.toggle.isOn = true;
        } else 
        {
            Debug.Log("ToggleAllCardTypes OFF");
            allCardsToggle.interactable = true;
        }
    }

    void ToggleCardType(Toggle toggle, CardType cardType)
    {
        if(toggle.isOn){
            Debug.Log(string.Format("Toggle{0} ON", cardType));
            activeCardTypes.Add(toggle);
        } else 
        {
            if(activeCardTypes.Count == 1)
            {
                toggle.isOn = true;
                return;
            }
            Debug.Log(string.Format("Toggle{0} OFF", cardType));
            activeCardTypes.Remove(toggle);
            allCardsToggle.isOn = false;
        }
    }

    void SetFactionToggleColor(Toggle toggle, Faction faction)
    {
        Color factionColor = ResearchManager.GetFactionColor(faction);
        if(toggle.isOn)
        {
            toggle.targetGraphic.color = factionColor;
        } else
        {
            Color newColor = new Color(factionColor.r, factionColor.g, factionColor.b, 100f/255f);
            toggle.targetGraphic.color = newColor;
        }
        
    }

    void EnterSingleFactionView()
    {
        Debug.Log(string.Format("[{0}] active toggles", activeFactions.Count));
        foreach(FactionToggle factionToggle in activeFactions)
        {
            Debug.Log(string.Format("Toggle{0} single faction view", factionToggle.faction));

            filterBackground.color = ResearchManager.GetFactionColor(factionToggle.faction);
        }
        
        singleFactionView = true;
    }

    void LeaveSingleFactionView()
    {
        Debug.Log("Leaving SingleFactionView");
        singleFactionView = false;
        filterBackground.color = DEFAULT_FILTER_COLOR;
    }
    


}
