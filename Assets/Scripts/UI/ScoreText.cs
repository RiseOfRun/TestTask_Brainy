using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        text.text = $"{GameController.Instance.FirstPlayer.Score}:{GameController.Instance.SecondPlayer.Score}";
    }
}
