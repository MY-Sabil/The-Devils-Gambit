using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    TextMeshProUGUI timerTxt;
    public float seconds;
    int minutes;

    public bool timerPaused = false;

    GameManager gameManager;
    float scoreTimer;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        timerTxt = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!timerPaused)
        {
            seconds += Time.deltaTime;
        }

        scoreTimer += Time.deltaTime;
        if(scoreTimer >= 20f)
        {
            scoreTimer = 0f;
            FindObjectOfType<SoundManager>().Play("minus-score");
            gameManager.Score -= 10;
        }

        if (seconds >= 59)
        {
            seconds = 0;
            minutes++;
        }
        timerTxt.text = minutes.ToString("00") + ":" + Mathf.Round(seconds).ToString("00");
    }
}
