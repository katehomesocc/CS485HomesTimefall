using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public Deck targetDeck;
    public Button drawButton;
    public Button shuffleButton;
    public RectTransform  handPanel;

    public GameObject agentCardDisplay;
    public GameObject essenceCardDisplay;
    public GameObject eventCardDisplay;
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
		Debug.Log ("Attempt to draw!");
        Card card = targetDeck.Draw();

        if(card == null){ return;}

        Debug.Log ("Got card");

        CardType cardType = card.cardType;

        Debug.Log ("Card type: " + cardType);

        GameObject obj = null;

        switch(cardType) 
        {
            case CardType.AGENT:
                obj = Instantiate(agentCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                AgentCardDisplay agentDC = obj.GetComponent<AgentCardDisplay>();

                if(agentDC == null)
                {
                    Debug.Log ("No Agent display :(");
                    return;
                } 

                Debug.Log ("Setting Agent display :)");
                agentDC.SetCard(card);

                break;
            case CardType.ESSENCE:
                obj = Instantiate(essenceCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                break;
            case CardType.EVENT:
                obj = Instantiate(eventCardDisplay, new Vector3(0, 0, 0), Quaternion.identity);
                EventCardDisplay eventDC = obj.GetComponent<EventCardDisplay>();

                if(eventDC == null)
                {
                    Debug.Log ("No Event display :(");
                    return;
                } 
                Debug.Log ("Setting Event display :)");
                eventDC.SetCard(card);
                break;
            default:
            //Error handling
                Debug.Log ("Invalid Card Type: " + cardType);
                return;
        }

        obj.transform.SetParent(transform);
        obj.transform.localScale =  new Vector3(0.55f, 0.55f, 0.55f);
        
	}

    void ShuffleDeck(){
		Debug.Log ("Attempt to shuffle!");
	}

}
