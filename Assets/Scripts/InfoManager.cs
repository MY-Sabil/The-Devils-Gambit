using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour
{
    [SerializeField] GameObject howTo, score, shop, recolor, reroll, story;

    public void HowToPlay()
    {
        FindObjectOfType<SoundManager>().Play("click3");
        howTo.SetActive(true);
        score.SetActive(false);
        shop.SetActive(false);
        recolor.SetActive(false);
        reroll.SetActive(false);
        story.SetActive(false);
    }
    public void HowScore()
    {
        FindObjectOfType<SoundManager>().Play("click3");
        howTo.SetActive(false);
        score.SetActive(true);
        shop.SetActive(false);
        recolor.SetActive(false);
        reroll.SetActive(false);
        story.SetActive(false);
    }
    public void HowShop()
    {
        FindObjectOfType<SoundManager>().Play("click3");
        howTo.SetActive(false);
        score.SetActive(false);
        shop.SetActive(true);
        recolor.SetActive(false);
        reroll.SetActive(false);
        story.SetActive(false);
    }
    public void HowRecolor()
    {
        FindObjectOfType<SoundManager>().Play("click3");
        howTo.SetActive(false);
        score.SetActive(false);
        shop.SetActive(false);
        recolor.SetActive(true);
        reroll.SetActive(false);
        story.SetActive(false);
    }
    public void HowReroll()
    {
        FindObjectOfType<SoundManager>().Play("click3");
        howTo.SetActive(false);
        score.SetActive(false);
        shop.SetActive(false);
        recolor.SetActive(false);
        reroll.SetActive(true);
        story.SetActive(false);
    }
    public void WhatStory()
    {
        FindObjectOfType<SoundManager>().Play("click3");
        howTo.SetActive(false);
        score.SetActive(false);
        shop.SetActive(false);
        recolor.SetActive(false);
        reroll.SetActive(false);
        story.SetActive(true);
    }
}
