using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public bool hasItem = false;
    public bool canGetOp = false;
    public bool canGetNum = false;
    public bool canGet = true;
    public bool isOp = false;
    public bool isShopItem = false;
    public bool isMenuItem = false;
    public string ItemText;
    [HideInInspector] public Vector2 itemPos;

    [HideInInspector] public GameObject item;

    public void OnDrop(PointerEventData eventData)
    {
        isOp = eventData.pointerDrag.GetComponent<DragNDrop>().isOperator;
        isShopItem = eventData.pointerDrag.GetComponent<DragNDrop>().isStoreItem;
        if (isOp && canGetOp)
        {
            item = eventData.pointerDrag;
            ItemText = eventData.pointerDrag.GetComponent<TextMeshProUGUI>().text;
            itemPos = eventData.pointerDrag.GetComponent<DragNDrop>().originalPos;
            Invoke("turnHasItemTrue", 0.2f);
        }
        else if(!isOp && !isShopItem && canGetNum)
        {
            item = eventData.pointerDrag;
            ItemText = eventData.pointerDrag.GetComponent<TextMeshProUGUI>().text;
            itemPos = eventData.pointerDrag.GetComponent<DragNDrop>().originalPos;
            Invoke("turnHasItemTrue", 0.2f);
        }
        else if (isMenuItem)
        {
            item = eventData.pointerDrag;
            ItemText = eventData.pointerDrag.GetComponent<TextMeshProUGUI>().text;
            Invoke("turnHasItemTrue", 0.2f);
        }
    }

    void turnHasItemTrue()
    {
        hasItem = true;
    }

    public void DestroyItem()
    {
        Destroy(item);
    }
}
