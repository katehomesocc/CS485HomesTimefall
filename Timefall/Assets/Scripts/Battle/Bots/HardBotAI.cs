using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardBotAI : BotAI
{
    protected override void AnalyzeBoard()
    {
        Debug.Log("Bot is analyzing the board.");

        currentState = BotState.ChooseAction;
    }

    protected override void ChooseAction()
    {
        Debug.Log("Bot is choosing an action.");

        Debug.Log("No valid actions. Ending turn.");
        currentState = BotState.EndTurn;
    }

    protected override IEnumerator ExecuteAction()
    {
        yield return null;
    }
}
