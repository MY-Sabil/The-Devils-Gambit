using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuTransition : MonoBehaviour
{
    [SerializeField] GameObject loaderCanvas;

    [SerializeField] CanvasGroup canvasGroup;

    public bool isToCredits, isToInfo;

    private void OnEnable()
    {
        if (isToCredits || isToInfo)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        canvasGroup.alpha = 0;
        canvasGroup.LeanAlpha(1, 0.15f);
        Invoke("changeScene", 0.16f);
    }

    void changeScene()
    {
        if (isToInfo)
        {
            SceneManager.LoadScene(2);
            return;
        }
        if (isToCredits)
        {
            SceneManager.LoadScene(3);
            return;
        }
        SceneManager.LoadScene(1);
    }

}
