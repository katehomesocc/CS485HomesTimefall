using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShieldDisplay : MonoBehaviour
{
    Shield shield;

    public RawImage shieldImage;
    public RawImage expirationState;

    public TMP_Text expirationText;

    public int turnCyclesLeft = 4;

    [Header("Turn Cycle Textures")]
    public Texture[] countdownTextures = new Texture[5];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResolveStartOfTurn()
    {
        if(shield.expiration == Expiration.NEXT_TURN){
            DecreaseCountDown();
        }
    }

    public void ResolveEndOfTurn()
    {

    }

    public void SetShield(Shield actionShield)
    {
        gameObject.SetActive(true);

        shield = actionShield;

        turnCyclesLeft = 4;

        SetExpiration(shield.expiration);
        
        SetColor(BattleManager.GetFactionColor(shield.owner.faction));
    }

    public void Expire()
    {
        SetExpiration(shield.expiration);
        
        shield = null;

        //TODO ANIMATION

        //TODO AUDIO FX

        gameObject.SetActive(false);

    }

    public void DecreaseCountDown()
    {

        turnCyclesLeft--;
        SetExpiration(shield.expiration);
    }


    void SetExpiration(Expiration expiration)
    {
        //TODO AUDIO FX
        expirationState.texture = countdownTextures[turnCyclesLeft];

        // if(turnCyclesLeft == 0)
        // {
        //     expirationText.text = "0";
        // }

        switch(expiration)
        {
            case Expiration.NONE:
                expirationText.text = "∞";
                expirationText.enabled = true;
                break;
            case Expiration.NEXT_TURN:
                // expirationText.text = "1";
                expirationText.enabled = false;
                break;
            default:
            expirationText.enabled = false;
                break;
        }
    }

    void SetColor(Color color)
    {
        expirationState.color = color;
        shieldImage.color = color;
    }

}