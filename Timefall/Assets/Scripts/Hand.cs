using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public static string LOCATION = "HAND";
    public Deck targetDeck;
    public Button drawButton;
    public Button shuffleButton;
    public RectTransform  handPanel;

    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;

    public Transform expandedViewTransform;
    // Start is called before the first frame update
    void Start()
    {
        Button drawBtn = drawButton.GetComponent<Button>();
		drawBtn.onClick.AddListener(DrawFromDeck);

        Button shuffleBtn = shuffleButton.GetComponent<Button>();
		shuffleBtn.onClick.AddListener(ShuffleDeck);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DrawFromDeck(){
        Card card = targetDeck.Draw();

        if(card == null){ return;}

        CardType cardType = card.cardType;

        Debug.Log (card.ToString());

        GameObject obj = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                if(agentDC == null){return;} 
                agentDC.SetCard(card);
                agentDC.InstantiateInHand(transform);

                break;
            case CardType.ESSENCE:
                obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

                if(essenceDC == null){return;} 
                essenceDC.SetCard(card);
                essenceDC.InstantiateInHand(transform);
                break;
            case CardType.EVENT:
                obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return;} 
                eventDC.SetCard(card);
                eventDC.InstantiateInHand(transform);
                break;
            default:
            //Error handling
                Debug.Log ("Invalid Card Type: " + cardType);
                return;
        }

        // obj.transform.SetParent(transform);
        //obj.transform.localScale =  new Vector3(0.55f, 0.55f, 0.55f);
        
	}

    void ShuffleDeck(){
        targetDeck.Shuffle();
	}

    public void ExpandCardView(Card card)
    {
        if(card == null){ return;}

        CardType cardType = card.cardType;

        Debug.Log (card.ToString());

        GameObject obj = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                if(agentDC == null){return;} 
                agentDC.SetCard(card);
                agentDC.Place(expandedViewTransform, "EXPAND");

                break;
            case CardType.ESSENCE:
                obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EssenceCardDisplay essenceDC = obj.GetComponent<EssenceCardDisplay>();

                if(essenceDC == null){return;} 
                essenceDC.SetCard(card);
                essenceDC.Place(expandedViewTransform, "EXPAND");
                break;
            case CardType.EVENT:
                obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null){return;} 
                eventDC.SetCard(card);
                eventDC.Place(expandedViewTransform, "EXPAND");
                break;
            default:
            //Error handling
                Debug.Log ("Invalid Card Type: " + cardType);
                return;
        }

        obj.transform.SetParent(expandedViewTransform, false);

    }

    public void CloseExpandCardView()
    {
        while (expandedViewTransform.childCount > 0) {
            DestroyImmediate(expandedViewTransform.GetChild(0).gameObject);
        }
    }

}
