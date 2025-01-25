using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatLogManager : MonoBehaviour
{
    public static ChatLogManager Instance;
    [SerializeField]
    private ScrollRect logView;
    public GameObject oneLinePrefab;
    public GameObject twoLinePrefab;

    public GameObject content;

    void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    // Start is called before the first frame update
    void Start()
    {
        // ChatMessageData messageData = new ChatMessageData(BattleManager.Instance.stewardPlayer, ChatMessageData.Action.TESTING);
        // AddMessage(messageData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendMessage(ChatMessageData messageData)
    {
        //TODO aduio?

        InstaniateMessage(messageData, GetPrefabForAction(messageData));
    }
    GameObject GetPrefabForAction(ChatMessageData messageData)
    {
        return messageData.SingleLineMessage() ? oneLinePrefab : twoLinePrefab;
    }

    void InstaniateMessage(ChatMessageData messageData, GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        
        ChatMessage chatMessage = obj.GetComponent<ChatMessage>();
        chatMessage.SetData(messageData);

        obj.transform.SetParent(content.transform, false);
    }

    void AddMessage(ChatMessageData messageData)
    {
        SendMessage(messageData);
        logView.verticalNormalizedPosition = 0;
    }


}
