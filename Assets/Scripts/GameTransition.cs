using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTransition : MonoBehaviour
{
    [SerializeField] GameObject loaderCanvas;

    [SerializeField] CanvasGroup canvasGroup;

    bool alreadyDone = false;

    private void OnEnable()
    {
        if (!alreadyDone)
        {
            canvasGroup.alpha = 1;
            canvasGroup.LeanAlpha(0, 0.15f);
            Invoke("Deactivate", 0.16f);
            alreadyDone = true;
        }
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void OutTransition()
    {
        canvasGroup.alpha = 0;
        canvasGroup.LeanAlpha(1, 0.2f);
        Invoke("ChangeScene", 0.2f);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }

}
