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

	private void Awake()
	{
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
		effectVolume = volume;
		UpdateVolume();
	}
	
}