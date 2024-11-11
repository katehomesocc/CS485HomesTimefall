using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FabricOfTimeRound : MonoBehaviour
{
    public Faction winner = Faction.NONE;
    public RawImage stewardsPeg;
    public RawImage seekersPeg;
    public RawImage sovereignsPeg;
    public RawImage weaversPeg;
    public void SetWinner(Faction faction)
    {
        winner = faction;

        if(winner == Faction.NONE)
        {
            //TODO: handle tie
            Debug.LogWarning("TODO: handle a tie");
            return;
        }

        RawImage winnerImage = GetFactionPeg(faction);

        SetImageAlphaOn(winnerImage);

    }

    RawImage GetFactionPeg(Faction faction)
    {

        switch (faction)
        {   
            case Faction.STEWARDS:
                return stewardsPeg;
            case Faction.SEEKERS:
                return seekersPeg;
            case Faction.SOVEREIGNS:
                return sovereignsPeg;
            case Faction.WEAVERS:
                return weaversPeg;
            default:
                return null;
        }
    }

    void SetImageAlphaOn(RawImage image)
    {
        Color tempColor = image.color;
        tempColor.a = 255f;
        image.color = tempColor;
    }
}
