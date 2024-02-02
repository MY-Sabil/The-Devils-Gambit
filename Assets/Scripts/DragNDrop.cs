using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class DragNDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
    Canvas canvas;

    RectTransform rectTransform;
    CanvasGroup canvasGroup;

    [HideInInspector] public Vector2 originalPos;

    public bool isOperator;
    public bool CanChangeColor;

    public bool isStoreItem;
    public bool isRecolor;
    public bool isReroll;

    bool isFirstDrag;

    public bool isMenu = false;
    GameManager gameManager;
    RandomManager randomManager;

    [HideInInspector] public bool cannotDrag = false;

    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
        canvasGroup = GetComponent<CanvasGroup>();

        if (!isMenu)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            randomManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RandomManager>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (rectTransform.anchoredPosition == originalPos)
        {
            isFirstDrag = true;
            return;
        }
        isFirstDrag = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!cannotDrag)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!cannotDrag)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!isStoreItem)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            ItemSlot slot = null;
            RectTransform pos = null;
            bool hasAnItem = false;
            bool canGet = true;
            bool canGetOp = false;
            bool canGetNum = false;
            string itemTxt = "";

            foreach (var result in results)
            {
                slot = result.gameObject.GetComponent<ItemSlot>();
                pos = result.gameObject.GetComponent<RectTransform>();

                if (slot != null)
                {
                    hasAnItem = result.gameObject.GetComponent<ItemSlot>().hasItem;
                    canGet = result.gameObject.GetComponent<ItemSlot>().canGet;
                    canGetOp = result.gameObject.GetComponent<ItemSlot>().canGetOp;
                    canGetNum = result.gameObject.GetComponent<ItemSlot>().canGetNum;
                    itemTxt = result.gameObject.GetComponent<ItemSlot>().ItemText;
                    break;
                }
            }

            if (slot != null && !hasAnItem && canGet)
            {
                if (isOperator)
                {
                    if (canGetOp && !cannotDrag)
                    {
                        rectTransform.anchoredPosition = pos.anchoredPosition;
                        FindObjectOfType<SoundManager>().Play("click3");
                        OperatorOperations(itemTxt);
                        return;
                    }

                    FindObjectOfType<SoundManager>().Play("click2");
                    rectTransform.anchoredPosition = originalPos;
                    return;
                }
                else if (canGetNum)
                {
                    rectTransform.anchoredPosition = pos.anchoredPosition;
                    FindObjectOfType<SoundManager>().Play("click3");
                    return;
                }
                else if (isMenu)
                {
                    rectTransform.anchoredPosition = pos.anchoredPosition;
                    MenuSelect(eventData);
                    return;
                }
            }

            if (isOperator && !isFirstDrag)
            {
                Destroy(gameObject);
                return;
            }
        }

        else if (isStoreItem)
        {
            if (isRecolor)
            {
                ItemRecolor(eventData);
            }
            else if (isReroll)
            {
                ItemReroll(eventData);
            }
        }

        FindObjectOfType<SoundManager>().Play("click2");
        rectTransform.anchoredPosition = originalPos;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        ItemSlot slot = null;

        foreach (var result in results)
        {
            slot = result.gameObject.GetComponent<ItemSlot>();

            if (slot != null)
            {
                result.gameObject.GetComponent<ItemSlot>().hasItem = false;
            }
        }
    }

    void OperatorOperations(string txt)
    {
        GameObject[] prefab = gameManager.opPrefabs;
        Transform parent = gameManager.opParent;

        if(txt == "+")
        {
            var obj = Instantiate(prefab[0]);
            obj.transform.SetParent(parent);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponent<RectTransform>().anchoredPosition = originalPos;
        }
        else if (txt == "-")
        {
            var obj = Instantiate(prefab[1]);
            obj.transform.SetParent(parent);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponent<RectTransform>().anchoredPosition = originalPos;
        }
        else if (txt == "x")
        {
            var obj = Instantiate(prefab[3]);
            obj.transform.SetParent(parent);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponent<RectTransform>().anchoredPosition = originalPos;
        }
        else if (txt == "/")
        {
            var obj = Instantiate(prefab[2]);
            obj.transform.SetParent(parent);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponent<RectTransform>().anchoredPosition = originalPos;
        }
    }

    void ItemRecolor(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        DragNDrop dnd = null;
        TextMeshProUGUI text = null;

        foreach (var result in results)
        {
            dnd = result.gameObject.GetComponent<DragNDrop>();
            text = result.gameObject.GetComponent<TextMeshProUGUI>();

            if (dnd != null && text != null)
            { 
                if (dnd.CanChangeColor)
                {
                    Debug.Log("Color Changed");
                    FindObjectOfType<SoundManager>().Play("shop-item");

                    string num = text.text[15..];
                    string color = randomManager.RandomColorGen();
                    Color kalar = new Color(255, 0, 0, 1);
                    ColorUtility.TryParseHtmlString(color, out kalar);
                    text.text = "<color=" + color + ">" + num;
                    text.transform.GetChild(0).GetComponent<SpriteRenderer>().color = kalar;

                    gameManager.Score -= 80;
                }
                break;
            }
        }
    }

    void ItemReroll(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        DragNDrop dnd = null;
        TextMeshProUGUI text = null;

        foreach (var result in results)
        {
            dnd = result.gameObject.GetComponent<DragNDrop>();
            text = result.gameObject.GetComponent<TextMeshProUGUI>();

            if (dnd != null && text != null)
            {
                if (dnd.CanChangeColor)
                {
                    Debug.Log("Number Changed");
                    FindObjectOfType<SoundManager>().Play("shop-item");

                    string color = text.text[..15];
                    string num = UnityEngine.Random.Range(1, 10).ToString();
                    text.text = color + num;

                    gameManager.Score -= 100;
                }
                break;
            }
        }
    }


    void MenuSelect(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        ItemSlot slot = null;
        string itemTxt = "";

        foreach (var result in results)
        {
            slot = result.gameObject.GetComponent<ItemSlot>();

            if (slot != null)
            {
                itemTxt = result.gameObject.GetComponent<ItemSlot>().ItemText;

                FindObjectOfType<SoundManager>().Play("click3");
                if(itemTxt == "Play")
                {
                    GameObject sceneLoader = GameObject.FindGameObjectWithTag("SceneLoader");
                    sceneLoader.transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (itemTxt == "Credits")
                {
                    GameObject sceneLoader = GameObject.FindGameObjectWithTag("SceneLoader");
                    sceneLoader.transform.GetChild(0).GetComponent<MainMenuTransition>().isToCredits = true;
                    sceneLoader.transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (itemTxt == "Info")
                {
                    GameObject sceneLoader = GameObject.FindGameObjectWithTag("SceneLoader");
                    sceneLoader.transform.GetChild(0).GetComponent<MainMenuTransition>().isToInfo = true;
                    sceneLoader.transform.GetChild(0).gameObject.SetActive(true);
                }
                break;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
