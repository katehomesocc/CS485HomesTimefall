using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Transform returnParent = null;

    int returnSiblingIndex = 0;
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("OnBeginDrag");

        returnParent = this.transform.parent;
        returnSiblingIndex = this.transform.GetSiblingIndex();

        this.transform.SetParent(this.transform.parent.parent); //edit layer later

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");

        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("OnEndDrag");

        this.transform.SetParent(returnParent);   
        this.transform.SetSiblingIndex(returnSiblingIndex);

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
