using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatLogManager : MonoBehaviour
{
    public static ChatLogManager Instance { get; private set; }
    public static int MAX_LOG_MESSAGES = 100;
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] private RectTransform container = null;
    [SerializeField] private GameObject msgFab = null;
    [SerializeField] private bool addNewToTop = false;
    private Queue<GameObject> messages = new Queue<GameObject>();

    // ------------------------------------------------------------------------------------------------------------

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

    private void OnDestroy()
    {
        Instance = null;
    }

    // ------------------------------------------------------------------------------------------------------------

    public void ToggleVisible()
    {
        if (gameObject.activeSelf)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(AutoScroll());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ClearAllMesssages()
    {
        GameObject go;
        while (messages.Count > 0)
        {
            go = messages.Dequeue();
            Destroy(go);
        }
    }

    public void SendMessage(ChatMessageData messageData)
    {
        //TODO aduio?

        AddMessage(messageData);
    }

    public void AddMessage(ChatMessageData messageData)
    {
        GameObject go = Instantiate(msgFab, container);
        
        TMP_Text text = go.GetComponent<TMP_Text>();
        
        text.text = messageData.BuildMessageString();

        messages.Enqueue(go);

        if (addNewToTop)
        {
            go.transform.SetAsFirstSibling();
        }

        // remove older messages if there are too many
        if (messages.Count > MAX_LOG_MESSAGES)
        {
            go = messages.Dequeue();
            Destroy(go);
        }

        // auto-scroll
        if (gameObject.activeSelf)
        {
            StartCoroutine(AutoScroll());
        }
    }

    private IEnumerator AutoScroll()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = addNewToTop ? 1 : 0;
    }

    // ------------------------------------------------------------------------------------------------------------
}





//     void InstaniateMessage(ChatMessageData messageData)
//     {
//         GameObject obj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        
//         TMP_Text text = obj.GetComponent<TMP_Text>();
        
//         text.text = messageData.BuildMessageString();

//         obj.transform.SetParent(content.transform, false);
//     }


