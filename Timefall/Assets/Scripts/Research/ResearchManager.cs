using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager Instance;

    

    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;

    public ResearchScrollableDisplay researchDisplay;
    public ResearchCardSpawner spawner;

    [Header("Settings Panel")]
    public bool isSettingsOpen = false;
    public GameObject settingsPanel;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectsSlider;
    public AudioClip UI_EFFECT_SOUND;

    [Header("Colors")]
    public static Color COLOUR_SEEKERS = new Color(33f/255,197f/255,104f/255, 1f);
    public static Color COLOUR_SOVEREIGNS = new Color(255f/255,35f/255,147f/255, 1f);
    public static Color COLOUR_STEWARDS = new Color(24f/255,147f/255,248f/255, 1f);
    public static Color COLOUR_WEAVERS = new Color(97f/255,65f/255,172f/255, 1f);

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

            masterSlider.onValueChanged.AddListener(delegate {
                UpdateMasterVolume(masterSlider.value);
            });
            
            musicSlider.onValueChanged.AddListener(delegate {
                UpdateMusicVolume(musicSlider.value);
            });
            
            effectsSlider.onValueChanged.AddListener(delegate {
                UpdateEffectVolume(effectsSlider.value);
            });
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        spawner.SpawnInitialLayout();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void LoadTitleScene()
    {
        PlayUIEffect();
        GameManager.Instance.LoadTitleScene();
    }

    public void ToggleSettingPanel()
    {
        PlayUIEffect();

        if(isSettingsOpen)
        {
            settingsPanel.SetActive(false);
            isSettingsOpen = false;
        } else 
        {
            settingsPanel.SetActive(true);
            isSettingsOpen = true;
        }
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

    void PlayUIEffect()
    {
        AudioManager.Instance.Play(UI_EFFECT_SOUND);
    }

}
