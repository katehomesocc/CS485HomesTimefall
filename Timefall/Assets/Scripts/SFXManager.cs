using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXManager : MonoBehaviour
{
    AudioManager audioManager;

    [Header("Toggle Noises")]
    public AudioClip toggleOn;
    public AudioClip toggleOff;


    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayToggleNoise(Toggle toggle)
    {
        if(toggle.isOn)
        {
            audioManager.Play(toggleOn);
        }
        else
        {
            audioManager.Play(toggleOff);
        }
    }
}
