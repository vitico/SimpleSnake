using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void SinglePlayer()
    {
        play(1);
    }
    public void MultiPlayer()
    {
        play(2);
    }

    private void play(int players = 1)
    {
        PlayerPrefs.SetInt("PlayerCount", players);
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
