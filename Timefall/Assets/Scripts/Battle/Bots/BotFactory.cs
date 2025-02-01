using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotFactory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static BotAI LoadBot(int difficultyLevel, GameObject playerObj)
    {
        switch (difficultyLevel)
        {
            case 1:
                return playerObj.AddComponent(typeof(EasyBotAI)) as EasyBotAI;
            case 2:
                return playerObj.AddComponent(typeof(MediumBotAI)) as MediumBotAI;
            case 3:
                return playerObj.AddComponent(typeof(HardBotAI)) as HardBotAI;
            default:
                return playerObj.AddComponent(typeof(MediumBotAI)) as MediumBotAI;
        }
    }
}
