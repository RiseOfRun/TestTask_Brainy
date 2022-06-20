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

    public void PauseGame(bool canUnpause)
    {
        GameController controller = GameController.Instance;
        if (controller.State == GameController.GameStates.Pause && canUnpause)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = Time.fixedUnscaledDeltaTime;
            controller.State = GameController.GameStates.RoundIn;
            return;
        }

        if (controller.State == GameController.GameStates.RoundIn)
        {
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            controller.State = GameController.GameStates.Pause;
            return;
        }
    }
}
