using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect
{
    public PlayerEffectType type;
    public Player player;

    public PlayerEffect (PlayerEffectType effectType, Player effectPlayer)
    {
        this.type = effectType;
        this.player = effectPlayer;
    }
}
