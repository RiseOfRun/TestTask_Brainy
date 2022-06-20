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
        text.text = $"Score:\n{GameController.Instance.FirstCharacter.Score}:{GameController.Instance.SecondCharacter.Score}";
    }
}
