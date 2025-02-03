using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

	[Header("Volume Levels")]
	public float masterVolume = 1.0f;
	public float musicVolume = 0.75f;
	public float effectVolume = 0.75f;

	[Header("Audio Sources")]
	public AudioSource EffectsSource;
	public AudioSource MusicSource;

	// Random pitch adjustment range.
    [Header("Random pitch range")]
	public float LowPitchRange = .95f;
	public float HighPitchRange = 1.05f;

	[Header("UI Clips")]
	public AudioClip UI_OPEN_MENU_CLIP;
	public AudioClip UI_EXIT_MENU_CLIP;
	public AudioClip UI_SLIDER_UPDATE_CLIP;

	[Header("Music Clips")]
	public AudioClip MUSIC_TITLE;
	public AudioClip MUSIC_BATTLE;
	public AudioClip MUSIC_REASEARCH;
	public AudioClip MUSIC_TRANSITION;

	private void Awake()
	{
		LoadPlayerPrefs();
		
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
            DontDestroyOnLoad(transform.gameObject);
        } 

		UpdateMasterVolume(masterVolume);
		UpdateMusicVolume(musicVolume);
		UpdateEffectVolume(effectVolume);
	}

	// Play a single clip through the sound effects source.
	public void Play(AudioClip clip)
	{
		EffectsSource.clip = clip;
		EffectsSource.Play();
	}

	// Play a single clip through the music source.
	public void PlayMusic(AudioClip clip)
	{
		MusicSource.clip = clip;
		MusicSource.loop = true;
		MusicSource.Play();
	}

	// Play a random clip from an array, and randomize the pitch slightly.
	public void RandomSoundEffect(params AudioClip[] clips)
	{
		int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

		EffectsSource.pitch = randomPitch;
		EffectsSource.clip = clips[randomIndex];
		EffectsSource.Play();
	}

	void UpdateVolume()
	{
		EffectsSource.volume = effectVolume * masterVolume;
		MusicSource.volume = musicVolume * masterVolume;
	}

	public void UpdateMasterVolume(float volume)
	{
		masterVolume = volume;
		UpdateVolume();
	}

	public void UpdateEffectVolume(float volume)
	{
		effectVolume = volume;
		UpdateVolume();
	}

	public void UpdateMusicVolume(float volume)
	{
		musicVolume = volume;
		UpdateVolume();
	}

	public void PlayUIOpenMenu()
	{
		Play(UI_OPEN_MENU_CLIP);
	}
	
	public void PlayUIExitMenu()
	{
		Play(UI_EXIT_MENU_CLIP);
	}

	public void PlayUISliderUpdate()
	{
		Play(UI_SLIDER_UPDATE_CLIP);
	}

	void LoadPlayerPrefs()
	{
		masterVolume = PlayerPrefs.GetFloat($"masterVolume", 1.0f);
		musicVolume = PlayerPrefs.GetFloat($"musicVolume", 0.75f);
		effectVolume = PlayerPrefs.GetFloat($"effectVolume", 0.55f);
	}

	public void PlayTitleMusic()
	{
		PlayMusic(MUSIC_TITLE);
	}

	public void PlayBattleMusic()
	{
		PlayMusic(MUSIC_BATTLE);
	}

	public void PlayResearchMusic()
	{
		PlayMusic(MUSIC_REASEARCH);
	}
	public void PlayTransitionMusic()
	{
		PlayMusic(MUSIC_TRANSITION);
	}
}