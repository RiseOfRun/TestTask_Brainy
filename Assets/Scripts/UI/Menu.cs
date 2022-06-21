using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene("GameLevel");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void MenuButtonPress()
    {
        if (GameController.Instance.State!=GameController.GameStates.Pause)
        {
            GameController.Instance.PauseGame();
        }
    }
}