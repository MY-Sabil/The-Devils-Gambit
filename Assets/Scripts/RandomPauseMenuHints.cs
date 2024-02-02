using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomPauseMenuHints : MonoBehaviour
{
    string[] quotes = {
        "Are you pausing the game to cheat? :O",
        "Why did you pause? :/",
        "Hint: Try to match numbers with same colored adjacent numbers.",
        "Did you know this game has a cool story behind it? <br>Go to the Info tab to check it out.",
        "Hint: Check out the Info tab to get more scores.",
        "Hint: The money you have is also your hp. <br>Don't let it get to 0.",
        "Never Gonna Give You Up, Never Gonna Let You Down...",
        "Did you know, this game took around 1200 lines of code to make?",
        "Combining numbers is hard, isn't it?",
        "This is the greatest hint ever!",
        "Just assume that this is a good hint and get back to the game :v",
        "What if, just like this game we are also in a simulation.",
        "Error 404 : Hint Not Found",
        "Caution: Computer Malfunctioning. <br>Please return to the game.",
        "On a scale of 1-10 how would you rate this game?",
        ""
    };

    private void OnEnable()
    {
        int index = Random.Range(0, quotes.Length);
        GetComponent<TextMeshProUGUI>().text = quotes[index];
    }
}
