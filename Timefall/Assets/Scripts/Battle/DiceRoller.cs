using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiceRoller : MonoBehaviour
{
    public RawImage D4_IMAGE;
    public RawImage D6_IMAGE;

    [Header("D4 Textures")]
    public Texture D4_ONE_TEX;
    public Texture D4_TWO_TEX;
    public Texture D4_THREE_TEX;
    public Texture D4_FOUR_TEX;

    [Header("D6 Textures")]
    public Texture D6_ONE_TEX;
    public Texture D6_TWO_TEX;
    public Texture D6_THREE_TEX;
    public Texture D6_FOUR_TEX;
    public Texture D6_FIVE_TEX;
    public Texture D6_SIX_TEX;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int RollD6()
    {
        int roll = Random.Range(1,7);
        Debug.Log(string.Format("Rolling D6... [{}]", roll));
        return roll;
    }

}
