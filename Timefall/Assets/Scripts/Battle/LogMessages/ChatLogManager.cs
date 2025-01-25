using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatLogManager : MonoBehaviour
{
    public static ChatLogManager Instance;
    [SerializeField]
    private ScrollRect logView;
    public GameObject prefab;
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

        InstaniateMessage(messageData);
        logView.verticalNormalizedPosition = 0;
    }

    void InstaniateMessage(ChatMessageData messageData)
    {
        GameObject obj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        
        TMP_Text text = obj.GetComponent<TMP_Text>();
        
        text.text = messageData.BuildMessageString();

        obj.transform.SetParent(content.transform, false);
    }


}
