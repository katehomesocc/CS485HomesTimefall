using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchFilterManager : MonoBehaviour
{
    public static Color DEFAULT_FILTER_COLOR = new Color(188f/255,193f/255,187f/255, 1f);
    public static Color DEFAULT_HEADER_COLOR = new Color(145f/255, 178f/255, 188f/255, 1f);
    ResearchManager researchManager;
    ResearchCardSpawner spawner;
    ResearchScrollableDisplay researchDisplay;
    public bool singleFactionView = false;
    public RawImage filterBackground;

    [Header("Faction Filters")]
    public Toggle allFactionsToggle;
    public FactionToggle stewardsToggle;
    public FactionToggle seekersToggle;
    public FactionToggle sovereignsToggle;
    public FactionToggle weaversToggle;

    [Header("Faction Textures")]
    public Texture STEWARDS_BACK_TEX;
    public Texture SEEKERS_BACK_TEX;
    public Texture SOVEREIGNS_BACK_TEX;
    public Texture WEAVERS_BACK_TEX;
    public Texture DEFALT_BACK_TEX;

    [Header("Card Type Filters")]
    public Toggle agentsToggle;
    public Toggle essenceToggle;
    public Toggle eventsToggle;

    public HashSet<FactionToggle> activeFactions = new HashSet<FactionToggle>();
    
    // Start is called before the first frame update
    void Start()
    {
        researchManager = ResearchManager.Instance;
        spawner = researchManager.spawner;
        researchDisplay = researchManager.researchDisplay;

        activeFactions.Add(stewardsToggle);
        activeFactions.Add(seekersToggle);
        activeFactions.Add(sovereignsToggle);
        activeFactions.Add(weaversToggle);

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

        agentsToggle.onValueChanged.AddListener(delegate {
                ToggleCardType(agentsToggle, CardType.AGENT);
            });
        essenceToggle.onValueChanged.AddListener(delegate {
                ToggleCardType(essenceToggle, CardType.ESSENCE);
            });
        eventsToggle.onValueChanged.AddListener(delegate {
                ToggleCardType(eventsToggle, CardType.EVENT);
            });
    }

    void ToggleAllFactions()
    {
        if(allFactionsToggle.isOn){
            allFactionsToggle.interactable = false;
            stewardsToggle.toggle.isOn = true;
            seekersToggle.toggle.isOn = true;
            sovereignsToggle.toggle.isOn = true;
            weaversToggle.toggle.isOn = true;
        } else 
        {
            allFactionsToggle.interactable = true;
        }
    }

    void ToggleFaction(FactionToggle factionToggle)
    {
        Toggle toggle = factionToggle.toggle;
        Faction faction =  factionToggle.faction;
        if(toggle.isOn){
            if(singleFactionView)
            {
                LeaveSingleFactionView();
            }
            activeFactions.Add(factionToggle);
            spawner.AddActiveFaction(faction);
            
        } else 
        {
            if (activeFactions.Count == 2)
            {
                activeFactions.Remove(factionToggle);
                spawner.RemoveActiveFaction(faction);
                EnterSingleFactionView();
            }
            else if(activeFactions.Count == 1)
            {
                toggle.isOn = true;
                return;
            }
            else
            {
                activeFactions.Remove(factionToggle);
                spawner.RemoveActiveFaction(faction);
                allFactionsToggle.isOn = false;
            }
            
            
        }

        SetFactionToggleColor(toggle, faction);
    }

    void ToggleCardType(Toggle toggle, CardType cardType)
    {
        if(toggle.isOn){
            spawner.SelectCardType(cardType);
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
        foreach(FactionToggle factionToggle in activeFactions)
        {
            Color factionColor = ResearchManager.GetFactionColor(factionToggle.faction);

            factionToggle.SetTextColor(factionColor);
            factionToggle.toggle.interactable = false;

            researchDisplay.SetHeaderBackgroundColor(factionColor);
            researchDisplay.SetContentBackgroundTexture(GetFactionBackground(factionToggle.faction));
        }
        
        singleFactionView = true;
    }

    void LeaveSingleFactionView()
    {
        singleFactionView = false;
        
        foreach(FactionToggle factionToggle in activeFactions)
        {
            factionToggle.SetTextColor(Color.black);
            factionToggle.toggle.interactable = true;
        }

        researchDisplay.SetHeaderBackgroundColor(DEFAULT_HEADER_COLOR);
        researchDisplay.SetContentBackgroundTexture(DEFALT_BACK_TEX);
    }

    Texture GetFactionBackground(Faction faction)
    {
        switch (faction)
        {
            case Faction.STEWARDS:
                return STEWARDS_BACK_TEX;
            case Faction.SEEKERS:
                return SEEKERS_BACK_TEX;
            case Faction.SOVEREIGNS:
                return SOVEREIGNS_BACK_TEX;
            case Faction.WEAVERS:
                return WEAVERS_BACK_TEX;
            default:
                return DEFALT_BACK_TEX;
        }
    }

}
