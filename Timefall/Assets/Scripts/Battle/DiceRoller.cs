using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DiceRoller : MonoBehaviour
{
    public AudioClip diceRollSound;
    public AudioClip successSound;
    public AudioClip failureSound; 
    
    [Header("Popup")]
    public GameObject popup;
    public TMP_Text currentDice;
    public TMP_Text minRollNeeded;
    public TMP_Text subtext;
    public RawImage diceImage;
    public int diceAnimationCount;
    public float resultTime;
    bool isPopupOpen = false;

    [Header("Dice Textures")]

    public Texture[] d4Textures = new Texture[4];
    public Texture[] d6Textures = new Texture[6];
    public Texture[] d8Textures = new Texture[8];

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OpenPopup(string diceType, string minRoll)
    {
        if(isPopupOpen) {return;}

        currentDice.text = diceType;
        minRollNeeded.text = minRoll;
        subtext.text = "Rolling...";

        popup.SetActive(true);
        isPopupOpen = true;
    }

    void ClosePopup()
    {
        if(!isPopupOpen) {return;}

        popup.SetActive(false);
        isPopupOpen = false;
    }
    
    public void RollD4(int rollNeeded, AgentCard agent)
    {
        int roll = Random.Range(1,5);
        Debug.Log(string.Format("Rolling D4... [{0}]", roll));

        StartCoroutine(RollDice("D4", rollNeeded, roll, agent));
    }

    public void RollD6(int rollNeeded, AgentCard agent)
    {
        int roll = Random.Range(1,7);
        Debug.Log(string.Format("Rolling D6... [{0}]", roll));

        StartCoroutine(RollDice("D6", rollNeeded, roll, agent));
    }

    public void RollD8(int rollNeeded, AgentCard agent)
    {
        int roll = Random.Range(1,9);
        Debug.Log(string.Format("Rolling D8... [{0}]", roll));

        StartCoroutine(RollDice("D8", rollNeeded, roll, agent));
    }

    IEnumerator RollDice(string diceType, int rollNeeded, int result, AgentCard agent)
    {
        OpenPopup(diceType, rollNeeded.ToString());
        yield return StartCoroutine(AnimateDice(diceType, result));

        if(result >= rollNeeded)
        {
            AudioManager.Instance.Play(successSound);
            subtext.text = "successful :)";
            agent.SuccessCallback();
        } else
        {
            AudioManager.Instance.Play(failureSound);
            subtext.text = "unsuccessful :(";
            agent.FailureCallback();
        }

        yield return new WaitForSeconds(resultTime);
        ClosePopup();
    }

    IEnumerator AnimateDice(string diceType, int result)
    {
        Texture[] textures = GetTextures(diceType);

        AudioManager.Instance.Play(diceRollSound);

        //while animatimg, show random dice face, but dont repeat

        int lastRand = -1;

        for (int i = diceAnimationCount; i > 0; i--)
        {
            int rand = GenerateRandNotDupe(textures.Length, lastRand);
            diceImage.texture = textures[rand];

            yield return new WaitForSeconds(0.2f);
        }

        //show actual roll result on finish

        diceImage.texture = textures[result - 1];

        yield return new WaitForSeconds(1);
        
    }
    
    int GenerateRandNotDupe(int range, int lastRand)
    {
        int rand = Random.Range(0, range);
        if(rand == lastRand)
        {
            return GenerateRandNotDupe(range, lastRand);
        }

        return rand;
    }

    Texture[] GetTextures(string diceType)
    {
        switch(diceType)
        {
            case "D4":
                return d4Textures;
            case "D6":
                return d6Textures;
            case "D8":
                return d8Textures;
            default:
                return null;
        }

    }



}
