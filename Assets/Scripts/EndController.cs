using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndController : MonoBehaviour
{
    public Text txtInfo;

    public void Start()
    {
        var players = PlayerPrefs.GetInt("PlayerCount", 2);
        var score1 = PlayerPrefs.GetInt("Score1", 0);
        var score2 = PlayerPrefs.GetInt("Score2", 0);
        var msg = "Fin del juego!";
        for (int i = 0; i < players; i++)
        {
            msg += "\nPlayer " + (i + 1) + ": " + PlayerPrefs.GetInt("Score" + (i + 1), 0);
        }
        txtInfo.text = msg;

    }

    public void retry()
    {
        SceneManager.LoadScene("Game");
    }

    public void exit()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
