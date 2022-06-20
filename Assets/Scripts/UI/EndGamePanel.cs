using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePanel : MonoBehaviour
{
    public TMP_Text Text;
    [HideInInspector] public string Winner = "";

    private void OnEnable()
    {
        Text.text = $"{Winner} wins\n\nPlayer1\tPlayer2\n{GameController.Instance.FirstCharacter.Score}\t" +
                    $"{GameController.Instance.SecondCharacter.Score}";
    }

    public void Restart()
    {
        GameController.Instance.StartGame();
        gameObject.SetActive(false);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}