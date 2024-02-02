using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DialogueBoxPopUp : MonoBehaviour, IDragHandler
{

    [SerializeField] GameObject panel;
    Canvas canvas;
    RectTransform rectTransform;

    public enum TypeOfDialogBox { gameOverBox, pauseMenuBox, InfoBox, CreditsBox }
    public TypeOfDialogBox dialogBoxType;

    GameManager gameManager;
    [SerializeField] GameObject transition;

    void Start()
    {
        transform.localScale = Vector2.zero;

        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        if(dialogBoxType == TypeOfDialogBox.pauseMenuBox)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
    }

    void OnEnable()
    {
        Open();
    }

    public void Open()
    {
        panel.GetComponent<CanvasGroup>().alpha = 0;
        panel.GetComponent<CanvasGroup>().LeanAlpha(1, 0.2f);
        Invoke("openDialogue", 0.2f);
    }

    void openDialogue()
    {
        transform.LeanScale(new Vector2(108f, 108f), 0.5f).setEaseInOutQuart();
    }

    public void Close()
    {
        FindObjectOfType<SoundManager>().Play("click3");
        transform.LeanScale(Vector2.zero, 0.5f).setEaseInOutQuart();
        Invoke("ClosePanel", 0.55f);
    }

    void ClosePanel()
    {
        panel.GetComponent<CanvasGroup>().LeanAlpha(0, 0.2f);
        Invoke("DeactivePanel", 0.2f);
    }

    void DeactivePanel()
    {
        panel.SetActive(false);

        if (dialogBoxType == TypeOfDialogBox.gameOverBox)
        {
            ReturnToMenu();
        }
        else if(dialogBoxType == TypeOfDialogBox.pauseMenuBox)
        {
            gameManager.UnPauseGame();
        }
        else if (dialogBoxType == TypeOfDialogBox.InfoBox || dialogBoxType == TypeOfDialogBox.CreditsBox)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void ReturnToMenu()
    {
        if(dialogBoxType != TypeOfDialogBox.gameOverBox)
        {
            FindObjectOfType<SoundManager>().Play("click3");
        }
        else if(dialogBoxType == TypeOfDialogBox.gameOverBox)
        {
            FindObjectOfType<SoundManager>().Play("game-over");
        }
        transition.SetActive(true);
        transition.GetComponent<GameTransition>().OutTransition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(dialogBoxType != TypeOfDialogBox.InfoBox && dialogBoxType != TypeOfDialogBox.CreditsBox)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
}
