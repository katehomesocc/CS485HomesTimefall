using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotCursor : MonoBehaviour
{

    public RawImage image;
    public BotAI botAI; 
    public float timeToMove = 1.0f;
    public Vector3 startPosition;
    void Awake()
    {
        startPosition = transform.position;
    }

    public void SetBot(BotAI bot, Faction faction)
    {
        botAI = bot;
        image.color = BattleManager.GetFactionColor(faction);
        transform.position = startPosition;
    }

    public void SpawnAt(Vector3 worldPos)
    {
        transform.position = worldPos;
        image.enabled = true;
    }

    public IEnumerator MoveToPosition(Vector3 position)
    {
        var currentPos = this.transform.position;
        var t = 0f;
        while(t <= 1f)
        {
            t += Time.deltaTime / timeToMove;
            this.transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        this.transform.position = position;
    }

    public void Enable()
    {
        image.enabled = true;
    }
    public void Disable()
    {
        image.enabled = false;
    }
}
